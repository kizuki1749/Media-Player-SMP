using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Tsukikage.WinMM.MidiIO
{
    public enum EventType
    {
        NoteOff = 0x80,
        NoteOn = 0x90,
        PolyKeyPress = 0xA0,
        ControlChange = 0xB0,
        ProgramChange = 0xC0,
        ChannelPress = 0xD0,
        PichBend = 0xE0,

        SystemExclusive = 0xF0,

        SequenceNumber = 0xFF00,
        TextEvent = 0xFF01,
        CopyrightNotice = 0xFF02,
        TrackName = 0xFF03,
        InstrumentName = 0xFF04,
        LyricText = 0xFF05,
        MarkerText = 0xFF06,
        CuePoint = 0xFF07,

        MidiChannelPrefix = 0xFF20,
        EndOfTrack = 0xFF2F,

        TempoSetting = 0xFF51,
        SMTPEOffset = 0xFF54,
        TimeSigunature = 0xFF58,
        KeySigunature = 0xFF59,

        SequencerSpecificEvent = 0xFF7F,
    }

    public class MidiSequence
    {
        #region StreamReader
        static int ReadInt2(Stream s)
        {
            int a = 0;
            a = a << 8 | s.ReadByte();
            a = a << 8 | s.ReadByte();
            return a;
        }

        static int ReadInt3(Stream s)
        {
            int a = 0;
            a = a << 8 | s.ReadByte();
            a = a << 8 | s.ReadByte();
            a = a << 8 | s.ReadByte();
            return a;
        }

        static int ReadInt4(Stream s)
        {
            int a = 0;
            a = a << 8 | s.ReadByte();
            a = a << 8 | s.ReadByte();
            a = a << 8 | s.ReadByte();
            a = a << 8 | s.ReadByte();
            return a;
        }

        static int ReadIntX(Stream s)
        {
            int a = 0;
            while (true)
            {
                int c = s.ReadByte();
                a = a << 7 | (c & 0x7F);
                if ((c & 0x80) == 0)
                    break;
            }
            return a;
        }

        static string ReadText(Stream s)
        {
            int length = ReadIntX(s);
            byte[] buf = new byte[length];
            s.Read(buf, 0, length);
            return Encoding.GetEncoding(932).GetString(buf, 0, length);
        }

        static byte[] ReadSysEx(Stream s)
        {
            int length = ReadIntX(s);
            byte[] buf = new byte[length + 1];
            buf[0] = 0xF0; s.Read(buf, 1, length);
            return buf;
        }

        static void ReadUnknownMeta(Stream s)
        {
            int length = ReadIntX(s);
            byte[] buf = new byte[length];
            s.Read(buf, 0, length);
        }

        #endregion

        public string Title = null, Copyright = null;
        public int Tracks;
        public int TimeBase;

        public MidiSequence(Stream source)
        {
            Load(source);
        }

        public MidiSequence(string path)
        {
            using (FileStream fs = File.OpenRead(path))
            {
                Load(fs);
            }
        }

        void Load(Stream fs)
        {
            List<string> stringTable = new List<string>(256);
            List<byte[]> sysexTable = new List<byte[]>(256);
            List<MidiEvent> eventList = new List<MidiEvent>(65536);

            { // Read HEADER
                if (ReadInt4(fs) != 0x4D546864) /* MThd */
                {
                    byte[] aaa = new byte[256];
                    fs.Read(aaa, 0, 16); /* RIFF MIDI */
                    if (ReadInt4(fs) != 0x4D546864) /* MThd */
                    {
                        fs.Read(aaa, 0, 108); /* MAC BINARY */
                        if (ReadInt4(fs) != 0x4D546864) /* MThd */
                            throw new ArgumentException("SMF/RMIじゃないっぽい", "path");
                    }
                }

                int len = ReadInt4(fs);
                int fmt = ReadInt2(fs);
                Tracks = ReadInt2(fs);
                TimeBase = ReadInt2(fs);
            }

            for (int track = 0; track < Tracks; track++)
            {
                if (ReadInt4(fs) != 0x4D54726B) /* MTrk */
                    throw new ArgumentException("SMFが壊れてるか対応してないっぽです。", "path");

                int end = ReadInt4(fs) + (int)fs.Position;

                int currentPos = 0;
                int lastStatus = 0;
                while (fs.Position < end)
                {
                    currentPos += ReadIntX(fs); // deltaTime

                    int first = -1, second = -1, third = -1;

                    first = fs.ReadByte();
                    if ((first & 0xF0) == 0)
                    {
                        // running status
                        second = first;
                        first = lastStatus;
                    }

                    lastStatus = first;
                    int eventType = first & 0xF0;

                    if (eventType != 0xF0)
                    {
                        // short message
                        second = second != -1 ? second : fs.ReadByte();
                        if (eventType != 0xC0) third = fs.ReadByte();
                        else third = 0;
                        eventList.Add(new MidiEvent(currentPos, (EventType)eventType, first, second, third));
                    }
                    else if (first == 0xF0 || first == 0xF7)
                    {
                        // long message
                        byte[] ex = ReadSysEx(fs);
                        eventList.Add(new MidiEvent(currentPos, EventType.SystemExclusive, sysexTable.Count));
                        sysexTable.Add(ex);
                    }
                    else if (first == 0xFF)
                    {
                        // meta event
                        EventType metatype = (EventType)(0xFF00 | fs.ReadByte());
                        switch (metatype)
                        {
                            case EventType.TextEvent:
                            case EventType.CopyrightNotice:
                            case EventType.TrackName:
                            case EventType.LyricText:
                            case EventType.MarkerText:
                            case EventType.CuePoint:
                                string text = ReadText(fs);
                                eventList.Add(new MidiEvent(currentPos, metatype, stringTable.Count));
                                if ((EventType)metatype == EventType.CopyrightNotice) Copyright = text;
                                if ((EventType)metatype == EventType.TrackName && Title == null) Title = text;
                                stringTable.Add(text);
                                break;

                            case EventType.TempoSetting:
                                ReadIntX(fs);
                                eventList.Add(new MidiEvent(currentPos, metatype, ReadInt3(fs)));
                                break;
                            case EventType.TimeSigunature:
                                ReadIntX(fs);
                                eventList.Add(new MidiEvent(currentPos, metatype, ReadInt4(fs)));
                                break;
                            case EventType.KeySigunature:
                                ReadIntX(fs);
                                eventList.Add(new MidiEvent(currentPos, metatype, ReadInt2(fs)));
                                break;

                            default:
                                ReadUnknownMeta(fs);
                                break;
                        }
                    }
                }
            }

            // All Tracks was readed.
            Events = eventList.ToArray();
            MargeSort(Events, delegate (MidiEvent x, MidiEvent y) { return x.Position - y.Position; });
            StringTable = stringTable.ToArray();
            SysExTable = sysexTable.ToArray();
        }

        /// <summary>
        /// 入力されたノートオンイベントに対するノートオフの位置を検索する。
        /// </summary>
        /// <param name="noteOnEvent">noteOnEvent</param>
        /// <returns>位置</returns>
        public int GetNoteOffEventIndex(int noteOnEvent)
        {
            int i = noteOnEvent + 1;
            if (Events[noteOnEvent].EventType != EventType.NoteOn)
                new ArgumentException("NoteOnイベントのインデックスではないようです。", "noteOnEvent");

            int noteCh = Events[noteOnEvent].Channel;
            int noteNum = Events[noteOnEvent].NoteNumber;
            int noteVel = Events[noteOnEvent].NoteVelocity;
            for (; i < Events.Length; i++)
            {
                MidiEvent e = Events[i];
                if (e.EventType == EventType.NoteOff && e.Channel == noteCh && e.NoteNumber == noteNum)
                    return i;
                if (e.EventType == EventType.NoteOn && e.Channel == noteCh && e.NoteNumber == noteNum && e.NoteVelocity == 0)
                    return i;
            }
            return -1;
        }

        public MidiEvent[] Events;
        public String[] StringTable;
        public byte[][] SysExTable;

        static void MargeSort<T>(T[] target, Comparison<T> comparer)
        {
            T[] work = new T[target.Length];
            for (int i = 0; ; i++)
            {
                int n = 1 << i;
                int m = n * 2;
                for (int j = 0; ; j++)
                {
                    int p = m * j;
                    int p1 = p, e1 = p1 + n;
                    int p2 = e1, e2 = Math.Min(p2 + n, target.Length);
                    int seg = e2 - p1;

                    if (seg <= n) break;

                    for (int k = 0; k < seg; k++)
                    {
                        if (p2 >= e2) work[k] = target[p1++];
                        else if (p1 >= e1) work[k] = target[p2++];
                        else if (comparer(target[p1], target[p2]) <= 0) work[k] = target[p1++];
                        else work[k] = target[p2++];
                    }

                    Array.Copy(work, 0, target, p, seg);
                }
                if (n >= target.Length) break;
            }
        }
    }

    public struct MidiEvent
    {
        public MidiEvent(int position, EventType type, int data)
        {
            this.position = position;
            this.type = type;
            this.data = data;
            this.UserData = 0;

            if (this.type == EventType.NoteOn && NoteVelocity == 0)
            {
                this.data ^= 0x10;
                this.type = EventType.NoteOff;
            }

        }

        public MidiEvent(int position, EventType type, int first, int second, int third)
        {
            this.position = position;
            this.type = type;
            this.data = first | second << 8 | third << 16;
            this.UserData = 0;

            if (this.type == EventType.NoteOn && NoteVelocity == 0)
            {
                this.data ^= 0x10;
                this.type = EventType.NoteOff;
            }
        }

        int position;
        EventType type;
        int data;
        public int UserData;

        public int Position { get { return position; } }
        public EventType EventType { get { return type; } }

        public int Channel { get { return data & 0x0F; } }

        public int NoteNumber { get { return data >> 8 & 0xFF; } }
        public int NoteVelocity { get { return data >> 16 & 0xFF; } }

        public int ControlNumber { get { return data >> 8 & 0xFF; } }
        public int ControlValue { get { return data >> 16 & 0xFF; } }
        public int ProgramNumber { get { return data >> 8 & 0xFF; } }

        public int PichBend { get { return (data >> 1 & 0x3F80) | (data >> 16 & 0x7F); } }

        public int Tempo { get { return data; } }
        public double BPM { get { return 60000000.0 / data; } }

        public int TimeNum { get { return data >> 24 & 0xFF; } }
        public int TimeDenom { get { return data >> 16 & 0xFF; } }
        public int TimeBeat { get { return data >> 8 & 0xFF; } }

        public int ScaleSharps { get { return data >> 8 & 0xFF; } }
        public int ScaleMajor { get { return data & 0xFF; } }

        public int SysExIndex { get { return data; } }
        public int TextIndex { get { return data; } }

        public uint RawData { get { return (uint)data; } }

        public override string ToString()
        {
            string pos = "" + (position / 1920 + 1).ToString("D4") + "." + (position % 1920 / 480 + 1) + "." + (position % 480).ToString("D3") + " > ";
            switch (EventType)
            {
                case EventType.NoteOn: return pos + "NoteOn : Ch." + Channel + " N" + NoteNumber + " V" + NoteVelocity;
                case EventType.NoteOff: return pos + "NoteOff : Ch." + Channel + " N" + NoteNumber;
                case EventType.ProgramChange: return pos + "ProgChg : Ch." + Channel + " Prog." + ProgramNumber;
                case EventType.ControlChange: return pos + "CtrlChg : Ch." + Channel + " CC" + ControlNumber + ">" + ControlValue;
                case EventType.PichBend: return pos + "PichBend : Ch." + Channel + " PB." + PichBend;
                case EventType.ChannelPress: return pos + "ChannelPress : Ch." + Channel;
                case EventType.PolyKeyPress: return pos + "PolyKeyPress : Ch." + Channel + " N" + NoteNumber + " V" + NoteVelocity;

                case EventType.SystemExclusive: return pos + "SysEx : " + data;

                case EventType.TempoSetting: return pos + "Tempo : " + data + "μs/beat = BPM. " + BPM + ")";
                case EventType.TimeSigunature: return pos + "TimeSign : " + TimeNum + " / " + "2^" + TimeDenom;
                case EventType.KeySigunature: return pos + "KeySign : Sharps." + ScaleSharps + " " + (ScaleMajor == 1 ? "Major" : "Minor");
                case EventType.TextEvent: return pos + "Text : " + data;
                case EventType.TrackName: return pos + "TrackName : " + data;
                case EventType.CopyrightNotice: return pos + "Copyright : " + data;
                case EventType.InstrumentName: return pos + "InstName : " + data;
                case EventType.LyricText: return pos + "Lyric : " + data;
                case EventType.MarkerText: return pos + "Marker : " + data;
                case EventType.CuePoint: return pos + "CuePoint : " + data;
                default: return pos + EventType.ToString() + " : " + data;
            }
        }
    }
}
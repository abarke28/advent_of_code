using aoc.common;
using aoc.utils;
using System.Text;
using System.Text.Json;

namespace aoc.y2022.day_13
{
    // https://adventofcode.com/2022/day/13
    public class Day_13
    {
        private abstract class Packet : IComparable<Packet>
        {
            public int CompareTo(Packet? other)
            {
                return (this, other) switch
                {
                    (Packet _, null) => 1,
                    (null, Packet _) => -1,
                    (NumPacket l, NumPacket r) => l.Num.CompareTo(r.Num),
                    (ListPacket l, NumPacket r) => l.CompareTo(r.ToListPacket()),
                    (NumPacket l, ListPacket r) => l.ToListPacket().CompareTo(r),
                    (ListPacket l, ListPacket r) => CompareArrays(l, r),
                    _ => throw new Exception("Unexpected comparison case")
                };
            }

            public override string ToString()
            {
                var sb = new StringBuilder();

                if (this is NumPacket n)
                {
                    sb.Append(n.Num);
                    sb.Append(',');
                }
                else if (this is ListPacket l)
                {
                    sb.Append('[');

                    foreach (var packet in l.Packets)
                    {
                        sb.Append(packet.ToString());
                    }
                    sb.Append(']');
                }

                return sb.ToString();
            }

            private static int CompareArrays(ListPacket left, ListPacket right)
            {
                var leftCount = left.Packets.Count;
                var rightCount = right.Packets.Count;
                var deltaExists = leftCount != rightCount;

                for (var i = 0; i < Math.Max(left.Packets.Count, right.Packets.Count); i++)
                {
                    if (deltaExists && i == leftCount)
                    {
                        return -1;
                    }
                    else if (deltaExists && i == rightCount)
                    {
                        return 1;
                    }

                    var compareResult = left.Packets[i].CompareTo(right.Packets[i]);

                    if (compareResult == 0)
                    {
                        continue;
                    }

                    return compareResult;
                }

                return 0;
            }
        }

        private class NumPacket : Packet
        {
            public int Num { get; set; }

            public NumPacket(int num)
            {
                Num = num;
            }

            public ListPacket ToListPacket()
            {
                return new ListPacket(new List<Packet> { new NumPacket(Num) });
            }
        }

        private class ListPacket: Packet
        {
            public List<Packet> Packets { get; set; }

            public ListPacket(List<Packet> packets)
            {
                Packets = packets;
            }
        }

        private class PacketPair
        {
            public Packet Left { get; set; }
            public Packet Right { get; set; }

            public PacketPair(Packet left, Packet right)
            {
                Left = left;
                Right = right;
            }
        }

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_13/input.txt");

            var packets = GetPackets(lines);
            var packetPairs = packets.Chunk(2).Select(pp => new PacketPair(pp[0], pp[1]));

            var count = ComputeSumOfOrderedPacketPairIndices(packetPairs.ToList());
            Console.WriteLine(count);

            var newPacket1 = new ListPacket(new List<Packet> { new NumPacket(2) });
            var newPacket2 = new ListPacket(new List<Packet> { new NumPacket(6) });

            var newPacketList = packets.Append(newPacket1).Append(newPacket2).ToList();
            newPacketList.Sort();

            var newPacket1Index = newPacketList.IndexOf(newPacket1) + 1;
            var newPacket2Index = newPacketList.IndexOf(newPacket2) + 1;
            Console.WriteLine(newPacket1Index * newPacket2Index);
        }

        private static int ComputeSumOfOrderedPacketPairIndices(IList<PacketPair> packetPairs)
        {
            var count = 0;

            for (var i = 0; i < packetPairs.Count; i++)
            {
                //Console.WriteLine($"Comparing left packet \n{packetPairs[i].Left} \nto right packet \n{packetPairs[i].Right}  ");
                if (packetPairs[i].Left.CompareTo(packetPairs[i].Right) == -1)
                {
                    //Console.WriteLine("Packets WERE in order");
                    count += (i + 1);
                }
            }

            return count;
        }

        private static JsonElement ParseStringToJson(string value)
        {
            return JsonSerializer.Deserialize<JsonElement>(value);
        }

        private static Packet ParseJsonToPacket(JsonElement json)
        {
            return (json.ValueKind) switch
            {
                JsonValueKind.Number => new NumPacket(json.GetInt32()),
                JsonValueKind.Array => new ListPacket(json.EnumerateArray()
                                                          .Select(ja => ParseJsonToPacket(ja))
                                                          .ToList()),
                _ => throw new Exception("Unexpected json token type"),
            };
        }

        private static IEnumerable<Packet> GetPackets(IEnumerable<string> lines)
        {
            return lines.Where(l => !string.IsNullOrWhiteSpace(l))
                        .Select(l => ParseStringToJson(l))
                        .Select(j => ParseJsonToPacket(j));
        }
    }
}

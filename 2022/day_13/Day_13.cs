using aoc.common;
using aoc.utils;
using System.Text.Json;

namespace aoc.y2022.day_13
{
    // https://adventofcode.com/2022/day/13
    public class Day_13 : ISolver
    {
        private class Packet { }

        private class NumPacket : Packet
        {
            public int Num { get; set; }

            public NumPacket(int num)
            {
                Num = num;
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

            var newPacketList = packets.Append(newPacket1).Append(newPacket2);

            var orderedPackets = newPacketList.Order(Comparer<Packet>.Create(Compare)).ToList();

            var newPacket1Index = orderedPackets.IndexOf(newPacket1) + 1;
            var newPacket2Index = orderedPackets.IndexOf(newPacket2) + 1;
            Console.WriteLine(newPacket1Index * newPacket2Index);
        }

        private static int ComputeSumOfOrderedPacketPairIndices(IList<PacketPair> packetPairs)
        {
            var count = 0;

            for (var i = 0; i < packetPairs.Count(); i++)
            {
                if (Compare(packetPairs[i].Left, packetPairs[i].Right) == -1)
                {
                    count += (i + 1);
                }
            }

            return count;
        }

        // Need to use this singature that implements IComparator<Packet> for ordering.
        private static int Compare(Packet left, Packet right)
        {
            return (left, right) switch
            {
                (Packet _, null) => 1,
                (null, Packet _) => -1,
                (NumPacket l, NumPacket r) => l.Num.CompareTo(r.Num),
                (ListPacket l, NumPacket r) => Compare(l, new ListPacket(new List<Packet> { r })),
                (NumPacket l, ListPacket r) => Compare(new ListPacket(new List<Packet> { l }), r),
                (ListPacket l, ListPacket r) => CompareArrays(l, r),
                _ => throw new Exception("Unexpected comparison case")
            };
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

                var compareResult = Compare(left.Packets[i], right.Packets[i]);

                if (compareResult == 0)
                {
                    continue;
                }

                return compareResult;
            }

            return 0;
        }

        private static JsonElement ParseStringToJson(string value)
        {
            return JsonSerializer.Deserialize<JsonElement>(value);
        }

        private static Packet ParseJsonToPacket(JsonElement json)
        {
            return json.ValueKind == JsonValueKind.Number
                ? new NumPacket(json.GetInt32())
                : new ListPacket(json.EnumerateArray().Select(j => ParseJsonToPacket(j)).ToList());
        }

        private static IEnumerable<Packet> GetPackets(IEnumerable<string> lines)
        {
            return lines.Where(l => !string.IsNullOrWhiteSpace(l))
                        .Select(l => ParseStringToJson(l))
                        .Select(j => ParseJsonToPacket(j));
        }
    }
}

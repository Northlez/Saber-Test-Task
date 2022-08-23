using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaberTestTask.NodeList
{
    public class ListNode
    {


        public ListNode Previous { get; set; }
        public ListNode Next { get; set; }
        public ListNode Random { get; set; }
        public string Data { get; set; }

        public override string ToString()
        {
            return $"{Data}";
        }
    }

    public class ListRandom
    {
        public ListNode Head { get; set; }
        public ListNode Tail { get; set; }
        public int Count
        {
            get
            {
                int counter = 0;
                ListNode current = Head;
                while (current != null)
                {
                    counter++;
                    current = current.Next;
                }
                return counter;
            }
        }

        public void Serialize(Stream s)
        {

            List<ListNode> nodes = new List<ListNode>();
            ListNode current = Head;

            while (current != null)
            {
                nodes.Add(current);
                current = current.Next;
            }

            byte[] buffer;
            foreach (var node in nodes)
            {
                int offset = 0;
                buffer = SerializeNode(node.Data, nodes.IndexOf(node.Random));
                s.Write(buffer, offset, buffer.Length);
            }
        }

        public void Deserialize(Stream s)
        {
            List<ListNode> nodes = new List<ListNode>();
            List<int> references = new List<int>();

            ListNode nodeBuffer;
            int refBuffer = -1;

            while (DeserializeNode(s, out nodeBuffer, out refBuffer))
            {
                nodes.Add(nodeBuffer);
                references.Add(refBuffer);
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                refBuffer = references[i];
                nodeBuffer = nodes[i];
                if (refBuffer >= 0)
                    nodeBuffer.Random = nodes[refBuffer];

                if (i > 0)
                {
                    nodes[i - 1].Next = nodeBuffer;
                    nodeBuffer.Previous = nodes[i - 1];
                }
            }

            Head = nodes.FirstOrDefault();
            Tail = nodes.LastOrDefault();
        }

        private byte[] SerializeNode(string data, Int32 randomNodeIndex)
        {
            byte[] dataBinary = System.Text.Encoding.Default.GetBytes(data);

            int byteArraySize = sizeof(Int32) * 2 + dataBinary.Length;
            byte[] result = new byte[byteArraySize];

            BitConverter.GetBytes(randomNodeIndex).CopyTo(result, 0);
            BitConverter.GetBytes(dataBinary.Length).CopyTo(result, sizeof(Int32));
            dataBinary.CopyTo(result, sizeof(Int32) * 2);
            return result;
        }

        private bool DeserializeNode(Stream stream, out ListNode listNode, out Int32 randomNodeIndex)
        {
            BinaryReader binaryReader = new BinaryReader(stream);
            bool ret = false;
            if (stream.Position < stream.Length)
            {
                ret = true;
                randomNodeIndex = binaryReader.ReadInt32();
                int dataSize = binaryReader.ReadInt32();
                string dataText;
                if (dataSize > 0)
                {
                    byte[] binaryDataText = binaryReader.ReadBytes(dataSize);
                    dataText = System.Text.Encoding.Default.GetString(binaryDataText);
                }
                else
                    dataText = null;

                listNode = new ListNode() { Data = dataText };
            }
            else
            {
                listNode = null;
                randomNodeIndex = 0;
            }
            return ret;
        }
    }

    public static class ListRandomExtension
    {
        public static void Insert(this ListRandom list, ListNode insertAfter, ListNode newItem)
        {
            ListNode prevNext;
            if (insertAfter != null)
            {
                prevNext = insertAfter.Next;
                insertAfter.Next = newItem;
                newItem.Previous = insertAfter;
            }
            else
            {
                prevNext = null;
                list.Head = newItem;
            }

            if (prevNext != null)
                prevNext.Previous = newItem;
            else
                list.Tail = newItem;
        }

        public static IEnumerable<ListNode> GetEnumerator(this ListRandom listRandom, bool fromTail = false)
        {
            ListNode next = fromTail ? listRandom.Tail : listRandom.Head;
            while (next != null)
            {
                yield return next;
                next = fromTail ? next.Previous : next.Next;
            }
        }
    }
}


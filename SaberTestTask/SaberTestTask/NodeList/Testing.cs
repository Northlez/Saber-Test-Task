using System;
using SaberTestTask.NodeList;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SaberTestTask
{
    class Program
    {
        static private ListNode listNode1;
        static private ListNode listNode2;
        static private ListNode listNode3;

        private const string FIRST = "first";
        private const string SECOND = "second";
        private const string THRID = "thrid";

        static void Main(string[] args)
        {
            listNode1 = new ListNode() { Data = FIRST };
            listNode1.Next = listNode2 = new ListNode() { Previous = listNode1, Data = SECOND };
            listNode2.Next = listNode3 = new ListNode() { Previous = listNode2, Data = THRID };

            ListRandom listRandom = new ListRandom() { Head = listNode1, Tail = listNode3 };
            List<ListNode> nodes = listRandom.GetEnumerator().ToList();
            Console.WriteLine(listRandom.Count);
            var fileStream = new FileStream("testSerialize", FileMode.Create, FileAccess.ReadWrite);

            listRandom.Serialize(fileStream);
            fileStream.Position = 0;

            listRandom.Deserialize(fileStream);         
            IEnumerable<ListNode> enumerator = listRandom.GetEnumerator();
            var newListRandom = enumerator.ToList();
            foreach (ListNode listNode in newListRandom)
            {
                Console.WriteLine(listNode.Data + "\n");
            }
            Console.ReadLine();
        }
    }
}

using KTreeDataTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace KTree
{
    /// <summary>
    /// Represents a node in a K-tree data structure.
    /// </summary>
    /// <typeparam name="T">The data type of the k-tree, must be derived from KTreeDataType.</typeparam>
    [Serializable]
    public class KTree<T> where T : KTreeDataType<T>
    {
        #region Properties
        /// <summary>
        /// Formatter to handle serialisation.
        /// </summary>
        private static BinaryFormatter Formatter { get; } = new();

        /// <summary>
        /// The order of the k-tree.
        /// </summary>
        private static int Order { get; set; }

        /// <summary>
        /// The root of the K-tree.
        /// </summary>
        private static KTree<T> Root { get; set; }

        /// <summary>
        /// An array of all observations to be inserted into the k-tree.
        /// </summary>
        private static Queue<T> Observations { get; set; }

        /// <summary>
        /// The parent k-tree node of the current instance.
        /// </summary>
        private KTree<T> Parent { get; set; }

        /// <summary>
        /// The keys (and their children) of the current instance.
        /// </summary>
        private List<KTreeKey<T>> KTreeKeys { get; set; }  
        #endregion

        #region Constructors
        /// <summary>
        /// Public constructor to instantiate the initial root of a K-tree.
        /// </summary>
        /// <param name="observations">The observations to be inserted into the k-tree.</param>
        /// <param name="order">The order of the k-tree.</param>
        public KTree(T[] observations, int order)
        {
            Order = order;
            Root = this;
            Observations = new(observations);
            KTreeKeys = new(Order);
        }

        /// <summary>
        /// Private constructor to instantiate a child node following a node split.
        /// </summary>
        /// <param name="parent">The node's parent.</param>
        /// <param name="kTreeKeys">The keys of the node.</param>
        protected internal KTree(KTree<T> parent, List<KTreeKey<T>> kTreeKeys)
        {
            Parent = parent;
            KTreeKeys = kTreeKeys;

            // If node keys have children, update parent property of children.
            if (KTreeKeys.Exists(kTreeKey => kTreeKey.Child != default))
            {
                foreach (KTreeKey<T> kTreeKey in KTreeKeys)
                {
                    kTreeKey.Child.Parent = this;
                }
            }
        }

        /// <summary>
        /// Private constructor to instantiate a parent node following a node split.
        /// </summary>
        /// <param name="key">The first key of the node.</param>
        /// <param name="child">The node's child.</param>
        private KTree(T key, KTree<T> child)
        {
            Root = this;
            KTreeKeys = new(Order)
            {
                new(key.Clone() as T, child)
            };
        }
        #endregion

        #region Public Methods
        public IEnumerable<KTreeKey<T>> ClusterMeansOf(T observation)
        {
            KTree<T> node = this;

            // While node is an internal node
            while (node.KTreeKeys.Exists(kTreeKey => kTreeKey.Child != default))
            {
                KTreeKey<T> nearest = node.GetNearest(observation);
                yield return nearest;
                node = nearest.Child;
            }
        }

        /// <summary>
        /// Constructs the k-tree by inserting all observations.
        /// </summary>
        /// <returns>The root of the entire k-tree.</returns>
        public KTree<T> Construct()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            while (Observations.TryDequeue(out T observation))
            {
                // Insert observation and update root
                Root.Insert(observation);
            }

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            return Root;
        }

#pragma warning disable SYSLIB0011 // Type or member is obsolete (Serialisation with BinaryFormatter)

        public static KTree<T> Open(string filePath)
        {
            using GZipStream gZipStream = new(File.OpenRead(filePath), CompressionMode.Decompress);
            return (KTree<T>)Formatter.Deserialize(gZipStream);
        }

        public void Print(int depth = 0)
        {
            string indent = "";

            for (int i = 1; i <= depth; i++)
            {
                indent += i > 1 ? "|  " : "   ";
            }

            Console.Write("{0}+- [", indent);

            for (int i = 0; i < KTreeKeys.Count; i++)
            {
                if (i < KTreeKeys.Count - 1)
                {
                    Console.Write("{0}, ", KTreeKeys[i].Key.ToString());
                }
                else
                {
                    Console.Write("{0}]\n", KTreeKeys[i].Key.ToString());
                    break;
                }
            }

            if (KTreeKeys.Exists(kTreeKey => kTreeKey.Child != default))
            {
                foreach (KTreeKey<T> kTreeKey in KTreeKeys)
                {
                    kTreeKey.Child.Print(depth + 1);
                }
            }


        }

        public void Save(string filePath)
        {
            using GZipStream gZipStream = new(File.OpenWrite(filePath), CompressionMode.Compress);
            Formatter.Serialize(gZipStream, this);
        }
#pragma warning restore SYSLIB0011
        #endregion

        #region Private Methods
        private KTreeKey<T> GetNearest(T observation)
        {

            double minimum = observation.DistanceFrom(KTreeKeys[0].Key);
            KTreeKey<T> nearest = KTreeKeys[0];

            for (int i = 1; i < KTreeKeys.Count; i++)
            {
                double distance = observation.DistanceFrom(KTreeKeys[i].Key);

                if ((distance < minimum) || (distance == minimum && KTreeKeys[i].Child.KTreeKeys.Count < nearest.Child.KTreeKeys.Count))
                {
                    minimum = distance;
                    nearest = KTreeKeys[i];
                }
            }

            return nearest;
        }

        private KTreeKey<T> ClusterMean()
        {
            return Parent.KTreeKeys.Find(kTreeKey => kTreeKey.Child == this);
        }

        private void Insert(T observation)
        {
            // If node is empty (is initial root node)
            if (KTreeKeys.Count == 0)
            {
                // Insert as first key amd create new root set its key to the same value
                KTreeKeys.Add(new KTreeKey<T>(observation));
                Parent = new(observation, this);
            }
            // Else node isn't empty (isn't initial root node)
            else
            {
                // If node has no children (is leaf node)
                if (KTreeKeys.Exists(kTreeKey => kTreeKey.Child == default))
                {
                    // Insert into first empty key
                    KTreeKeys.Add(new KTreeKey<T>(observation));

                    // If node is now full
                    if (KTreeKeys.Count == KTreeKeys.Capacity)
                    {
                        Split();
                    }
                    // Else node isn't full, if parent exists
                    else if (Parent != default)
                    {
                        UpdateParent();
                    }
                }
                // Else node has children (is root or internal node)
                else
                {
                    GetNearest(observation).Child.Insert(observation);
                }
            }
        }

        private void Insert(Cluster<T> cluster)
        {
            KTreeKeys.Add(new KTreeKey<T>(this, cluster.Mean, cluster.Observations));

            // If node is now full
            if (KTreeKeys.Count == KTreeKeys.Capacity)
            {
                Split();
            }
            // Else node isn't full, if parent exists
            else if (Parent != default)
            {
                UpdateParent();
            }
        }

        private void Split()
        {
            // Split node using k-means
            KMeans<T> kmeans = new(KTreeKeys);
            Cluster<T>[] clusters = kmeans.Partition();

            // Replace keys of current node with those of the first cluster
            KTreeKeys = clusters[0].Observations;

            // If parent exists
            if (Parent != default)
            {
                // Replace mean in parent with that of the first cluster
                ClusterMean().Key = clusters[0].Mean;
            }
            // Else no parent exists (is root)
            else
            {
                // Create new root and set mean to that of the first cluster
                Parent = new(clusters[0].Mean, this);
            }

            // Insert second cluster into parent
            Parent.Insert(clusters[1]);
        }

        private void UpdateParent()
        {
            KTreeKey<T> clusterMean = ClusterMean();
            clusterMean.Key.ClearProperties();

            foreach (KTreeKey<T> kTreeKey in KTreeKeys)
            {
                clusterMean.Key += kTreeKey.Key;
            }

            clusterMean.Key /= KTreeKeys.Count;

            if (Parent.Parent != default)
            {
                Parent.UpdateParent();
            }
        }
        #endregion
    }
}
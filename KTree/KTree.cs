using KTreeDataTypes;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

#pragma warning disable SYSLIB0011 // Type or member is obsolete

namespace KTree
{
    /// <summary>
    /// Represents a node in a k-Tree data structure.
    /// </summary>
    /// <typeparam name="T">The data type of the k-tree, must be derived from KTreeDataType.</typeparam>
    [Serializable]
    public class KTree<T> where T : KTreeDataType<T>
    {
        /// <summary>
        /// The order of the k-tree.
        /// </summary>
        public static int Order { get; set; }
        /// <summary>
        /// An array of all observations to be inserted into the k-tree.
        /// </summary>
        public static T[] Observations { get; set; }
        /// <summary>
        /// The parent k-tree node of the current instance.
        /// </summary>
        public KTree<T> Parent { get; set; }
        /// <summary>
        /// The keys (and their children) of the current instance.
        /// </summary>
        public KTreeKey<T>[] KTreeKeys { get; set; }
        private static BinaryFormatter Formatter { get; } = new();

        /// <summary>
        /// Public constructor to create the initial root of a k-tree.
        /// </summary>
        /// <param name="observations">The observations to be inserted into the k-tree.</param>
        /// <param name="order">The order of the k-tree.</param>
        public KTree(T[] observations, int order)
        {
            Order = order;
            Observations = observations;
            KTreeKeys = new KTreeKey<T>[Order];
        }

        public KTree(KTree<T> parent, KTreeKey<T>[] kTreeKeys)
        {
            Parent = parent;
            KTreeKeys = kTreeKeys;

            foreach (KTreeKey<T> kTreeKey in KTreeKeys)
            {
                if (kTreeKey != default && kTreeKey.Child != default)
                {
                    kTreeKey.Child.Parent = this;
                }
            }
        }

        /// <summary>
        /// Private constructor to create the parent of the first root node.
        /// </summary>
        /// <param name="firstKey">The first key of the node.</param>
        /// <param name="child">The node's child.</param>
        private KTree(T firstKey, KTree<T> child)
        {
            KTreeKeys = new KTreeKey<T>[Order];
            KTreeKeys[0] = new(firstKey.Clone() as T, child);
        }

        private KTree()
        {
            KTreeKeys = new KTreeKey<T>[Order];
        }

        /// <summary>
        /// Constructs the k-tree by inserting all observations.
        /// </summary>
        /// <returns>The root of the entire k-tree.</returns>
        public KTree<T> Construct()
        {
            // Root of entire k-tree
            KTree<T> root = this;

            foreach (T observation in Observations)
            {
                // Insert observation and update root
                root = root.Insert(observation);
            }

            return root;
        }

        public KTreeKey<T> FindMeanOf(T observation, int desiredDepth, int currentDepth = 0)
        {
            double?[] distances = new double?[KTreeKeys.Length];

            for (int i = 0; i < KTreeKeys.Length; i++)
            {
                if (KTreeKeys[i] != default && KTreeKeys[i].Key != default)
                {
                    distances[i] = observation.DistanceFrom(KTreeKeys[i].Key);
                }
                else
                {
                    break;
                }
            }

            if (currentDepth == desiredDepth)
            {
                return KTreeKeys[GetIndexOfMin(distances)];
            }
            else
            {
                if (KTreeKeys[GetIndexOfMin(distances)].Child != default)
                {
                    return KTreeKeys[GetIndexOfMin(distances)].Child.FindMeanOf(observation, desiredDepth, currentDepth + 1);
                }
                else
                {
                    return default;
                }
            }
        }

        public static KTree<T> Open(string filePath)
        {
            using GZipStream gZipStream = new(File.OpenRead(filePath), CompressionMode.Decompress);
            return (KTree<T>)Formatter.Deserialize(gZipStream);
        }

        public void Save(string filePath)
        {
            using GZipStream gZipStream = new(File.OpenWrite(filePath), CompressionMode.Compress);
            Formatter.Serialize(gZipStream, this);
        }

        private int GetIndexOfMin(double?[] distances)
        {
            double minimum = (double)distances[0];
            int minimumIndex = 0;

            for (int i = 1; i < distances.Length; i++)
            {
                if (distances[i].HasValue)
                {
                    if (distances[i] < minimum)
                    {
                        minimum = (double)distances[i];
                        minimumIndex = i;
                    }
                    // If more than one minimum
                    else if (distances[i] == minimum)
                    {
                        for (int j = 0; j < distances.Length; j++)
                        {
                            if (true)
                            {

                            }
                            // If second minimum belongs to a node with less children
                            if (KTreeKeys[i].Child.KTreeKeys[j] == default && KTreeKeys[minimumIndex].Child.KTreeKeys[j] != default)
                            {
                                minimum = (double)distances[i];
                                minimumIndex = i;
                            }
                        }
                    }
                }
                else
                {
                    break;
                }



            }

            return minimumIndex;
        }

        private int GetIndexInParent()
        {
            int i;

            for (i = 0; i < Parent.KTreeKeys.Length; i++)
            {
                if (Parent.KTreeKeys[i].Child == this)
                {
                    break;
                }
            }

            return i;
        }

        private KTree<T> GetRoot()
        {
            if (Parent == default)
            {
                return this;
            }
            else
            {
                return Parent.GetRoot();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="observation"></param>
        /// <returns></returns>
        private KTree<T> Insert(T observation)
        {
            // If node is empty (is initial root node)
            if (KTreeKeys[0] == default)
            {
                // Insert as first key amd create new root set its key to the same value
                KTreeKeys[0] = new KTreeKey<T>(observation);
                Parent = new(observation, this);
            }
            // Else node isn't empty (isn't initial root node)
            else
            {
                // If node has no children (is leaf node)
                if (KTreeKeys[0].Child == default)
                {
                    // Insert into first empty key
                    for (int i = 1; i < Order; i++)
                    {
                        if (KTreeKeys[i] == default)
                        {
                            KTreeKeys[i] = new KTreeKey<T>(observation);

                            // If node is now full
                            if (i == Order - 1)
                            {
                                Split();
                            }
                            // Else node isn't full, if parent exists
                            else if (Parent != default)
                            {
                                UpdateParent();
                            }

                            break;
                        }
                    }
                }
                // Else node has children (is root or internal node)
                else
                {
                    // If node has only one key (has only one child)
                    if (KTreeKeys[1] == default)
                    {
                        // Insert into only child
                        KTreeKeys[0].Child.Insert(observation);
                    }
                    // Else node has multiple keys (multiple children)
                    else
                    {
                        double?[] distances = new double?[Order];

                        for (int i = 0; i < KTreeKeys.Length; i++)
                        {
                            if (KTreeKeys[i] != default && KTreeKeys[i].Key != default)
                            {
                                distances[i] = observation.DistanceFrom(KTreeKeys[i].Key);
                            }
                            else
                            {
                                break;
                            }
                        }

                        KTreeKeys[GetIndexOfMin(distances)].Child.Insert(observation);
                    }
                }
            }

            // Return root of tree
            return GetRoot();
        }

        private void Split()
        {
            // Split node using k-means
            KMeans<T> kmeans = new(KTreeKeys);
            Cluster<T>[] clusters = kmeans.Partition();

            // Replace keys and mean of current node with those of the first cluster
            KTreeKeys = clusters[0].Observations;
            Parent.KTreeKeys[GetIndexInParent()].Key = clusters[0].Mean;

            // Insert second cluster into parent
            Parent.Insert(clusters[1]);
        }

        private void Insert(Cluster<T> cluster)
        {
            // Insert into first empty key
            for (int i = 1; i < KTreeKeys.Length; i++)
            {
                if (KTreeKeys[i] == default)
                {
                    KTreeKeys[i] = new KTreeKey<T>(this, cluster.Mean, cluster.Observations);

                    // If node is now full
                    if (i == KTreeKeys.Length - 1)
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
                            Parent.KTreeKeys[GetIndexInParent()].Key = clusters[0].Mean;
                        }
                        // Else no parent exists (is root)
                        else
                        {
                            // Create new root and set mean to that of the first cluster
                            Parent = new();
                            Parent.KTreeKeys[0] = new(clusters[0].Mean, this);
                        }

                        // Insert second cluster into parent
                        Parent.Insert(clusters[1]);
                    }
                    // Else node isn't full, if parent exists
                    else if (Parent != default)
                    {
                        UpdateParent();
                    }

                    break;
                }
            }
        }

        public void Print(int depth = 0)
        {
            string indent = "";

            for (int i = 1; i <= depth; i++)
            {
                indent += i > 1 ? "|  " : "   ";
            }

            Console.Write("{0}+- [", indent);

            for (int i = 0; i < KTreeKeys.Length; i++)
            {
                if (i != KTreeKeys.Length - 1 && KTreeKeys[i + 1] != default)
                {
                    Console.Write("{0}, ", KTreeKeys[i].Key.ToString());
                }
                else
                {
                    Console.Write("{0}]\n", KTreeKeys[i].Key.ToString());
                    break;
                }
            }

            for (int i = 0; i < KTreeKeys.Length; i++)
            {
                if (KTreeKeys[i] != default && KTreeKeys[i].Child != default)
                {
                    KTreeKeys[i].Child.Print(depth + 1);
                }
                else
                {
                    break;
                }
            }
        }

        private void UpdateParent()
        {
            int indexInParent = GetIndexInParent();
            int numKeys = 0;
            Parent.KTreeKeys[indexInParent].Key.ClearProperties();

            foreach (KTreeKey<T> kTreeKey in KTreeKeys)
            {
                if (kTreeKey != default && kTreeKey.Key != default)
                {
                    Parent.KTreeKeys[indexInParent].Key += kTreeKey.Key;
                    numKeys++;
                }
                else
                {
                    break;
                }
            }

            if (numKeys != 0)
            {
                Parent.KTreeKeys[indexInParent].Key /= numKeys;
            }

            if (Parent.Parent != default)
            {
                Parent.UpdateParent();
            }
        }
    }
}
#pragma warning restore SYSLIB0011 // Type or member is obsolete
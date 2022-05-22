using KTreeDataTypes;
using System.Collections.Generic;
using static RandomArray.RandomArray;

namespace KTree
{
    /// <summary>
    /// Represents a k-means clustering.
    /// </summary>
    /// <typeparam name="T">The fundamental data type of the clusters, must be derived from KTreeDataType.</typeparam>
    public class KMeans<T> where T : KTreeDataType<T>
    {
        /// <summary>
        /// The number of means (or clusters).
        /// </summary>
        public static int K { get; } = 2;
        /// <summary>
        /// The clusters of observations.
        /// </summary>
        public Cluster<T>[] Clusters { get; set; }
        /// <summary>
        /// The observations to be clustered.
        /// </summary>
        public List<KTreeKey<T>> Observations { get; set; }

        /// <summary>
        /// Public constructor to create a KMeans object.
        /// </summary>
        /// <param name="observations">The observations to be clustered.</param>
        public KMeans(List<KTreeKey<T>> observations)
        {
            Observations = observations;
            Clusters = new Cluster<T>[K];
            InitClusers();
        }

        /// <summary>
        /// Get the clusters by assigning observations to the nearest cluster and updating cluster means until convergence.
        /// </summary>
        /// <returns>An array of clusters.</returns>
        public Cluster<T>[] Partition()
        {
            AssignObservations();

            if (AssignmentsChanged())
            {
                UpdateMeans();
                Partition();
            }

            return Clusters;
        }

        private bool AssignmentsChanged()
        {
            foreach (Cluster<T> cluster in Clusters)
            {
                if (cluster.AssignmentChanged())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Assigns observations to the cluster with the nearest mean.
        /// </summary>
        private void AssignObservations()
        {
            foreach (KTreeKey<T> observation in Observations)
            {
                GetNearest(observation).AddKTreeKey(observation);
            }
        }
        
        private Cluster<T> GetNearest(KTreeKey<T> observation)
        {
            double minimum = observation.Key.DistanceFrom(Clusters[0].Mean);
            Cluster<T> nearest = Clusters[0];

            for (int i = 1; i < Clusters.Length; i++)
            {
                double distance = observation.Key.DistanceFrom(Clusters[i].Mean);

                if ((distance < minimum) || (distance == minimum && Clusters[i].Observations.Count < nearest.Observations.Count))
                {
                    minimum = distance;
                    nearest = Clusters[i];
                }
            }

            return nearest;
        }
        
        /// <summary>
        /// Initialise clusters by choosing random observations to be initial means.
        /// </summary>
        private void InitClusers()
        {
            int[] randomIndexes = GenerateRandomIntArray(0, Observations.Count - 1, K);

            for (int i = 0; i < K; i++)
            {
                Clusters[i] = new Cluster<T>(Observations[randomIndexes[i]].Key, Observations.Count);
            }
        }

        private void UpdateMeans()
        {
            foreach (Cluster<T> cluster in Clusters)
            {
                cluster.UpdateMean();
                cluster.ClearKeys();
            }
        }
    }
}

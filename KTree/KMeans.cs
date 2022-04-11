using KTreeDataTypes;
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
        public KTreeKey<T>[] Observations { get; set; }

        /// <summary>
        /// Public constructor to create a KMeans object.
        /// </summary>
        /// <param name="observations">The observations to be clustered.</param>
        public KMeans(KTreeKey<T>[] observations)
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

            bool assignmentsChanged = false;

            foreach (Cluster<T> cluster in Clusters)
            {
                if (cluster.AssignmentChanged())
                {
                    assignmentsChanged = true;
                    break;
                }
            }

            if (assignmentsChanged)
            {
                UpdateMeans();
                Partition();
            }

            return Clusters;
        }

        /// <summary>
        /// Assigns observations to the cluster with the nearest mean.
        /// </summary>
        private void AssignObservations()
        {
            foreach (KTreeKey<T> observation in Observations)
            {
                double[] distances = GetDistances(observation);
                Clusters[GetIndexOfMin(distances)].AddKTreeKey(observation);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="observation"></param>
        /// <returns></returns>
        private double[] GetDistances(KTreeKey<T> observation)
        {
            double[] distances = new double[K];

            for (int j = 0; j < K; j++)
            {
                distances[j] = observation.Key.DistanceFrom(Clusters[j].Mean);
            }

            return distances;
        }

        /// <summary>
        /// Gets the index of the minimal distance value in an array. If there are multiple minimums, use that of the cluster with the least observations.
        /// </summary>
        /// <param name="distances">An array of distance values.</param>
        /// <returns>The index of the minimal distance value.</returns>
        private int GetIndexOfMin(double[] distances)
        {
            double minimum = distances[0];
            int minimumIndex = 0;

            for (int i = 1; i < K; i++)
            {
                if (distances[i] < minimum)
                {
                    minimum = distances[i];
                    minimumIndex = i;
                }

                // If more than one minimum
                else if (distances[i] == minimum)
                {
                    for (int j = 0; j < Observations.Length; j++)
                    {
                        // If second minimum belongs to a cluster with less observations
                        if (Clusters[i].Observations[j] == default && Clusters[minimumIndex].Observations[j] != default)
                        {
                            minimum = distances[i];
                            minimumIndex = i;
                        }
                    }
                }
            }

            return minimumIndex;
        }

        /// <summary>
        /// Initialise clusters by choosing random observations to be initial means.
        /// </summary>
        private void InitClusers()
        {
            int[] randomIndexes = GenerateRandomIntArray(0, Observations.Length - 1, K);

            for (int i = 0; i < K; i++)
            {
                Clusters[i] = new Cluster<T>(Observations[randomIndexes[i]].Key, Observations.Length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

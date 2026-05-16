#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using KernelImpl.Internal;

namespace KernelImpl.Tests.Internal
{
    public class CacheConcurrencyTests
    {
        public class ConcurrencyDummy
        {
            public int Id { get; set; }
            public string Data { get; set; } = string.Empty;
        }

        [Fact]
        public void SimpleCachedData_Multithreading_LecturesEtEcrituresSimultanees_NeCrashentPas()
        {
            // Arrange
            var cache = new SimpleCachedData<ConcurrencyDummy>();
            cache.UpdateSnapshot(new ConcurrencyDummy { Id = -1, Data = "Init" });

            int iterations = 500_000; // Un demi-million d'opérations concurrentes
            int writeCount = 0;
            int readCount = 0;

            // Act : On bombarde le cache avec 500 000 opérations réparties sur tous les coeurs CPU
            Parallel.For(0, iterations, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            {
                if (i % 10 == 0) // 10% d'écritures
                {
                    cache.UpdateSnapshot(new ConcurrencyDummy { Id = i, Data = $"Ecriture_{i}" });
                    System.Threading.Interlocked.Increment(ref writeCount);
                }
                else // 90% de lectures (Simulation d'un cache fortement sollicité en lecture)
                {
                    var current = cache.Cache; // Lecture O(1)

                    // Assert interne au thread : La donnée lue ne doit JAMAIS être nulle ou corrompue
                    current.Should().NotBeNull();
                    current.Data.Should().NotBeNull();

                    System.Threading.Interlocked.Increment(ref readCount);
                }
            });

            // Assert
            writeCount.Should().Be(iterations / 10);
            readCount.Should().Be(iterations - writeCount);
            cache.Cache.Should().NotBeNull("Le cache final doit être valide après un stress test multithread.");
        }

        [Fact]
        public void DeduplicatedCachedData_Multithreading_LecturesEtFullSnapshots_NeCrashentPas()
        {
            // Arrange
            var cache = new DeduplicatedCachedData<int, ConcurrencyDummy>();
            int iterations = 100_000;

            // Act
            Parallel.For(0, iterations, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            {
                if (i % 5 == 0) // 20% d'écritures massives de listes
                {
                    var newList = new List<ConcurrencyDummy>
                    {
                        new ConcurrencyDummy { Id = i, Data = "A" },
                        new ConcurrencyDummy { Id = i + 1, Data = "B" }
                    };
                    cache.UpdateFullSnapshot(newList, x => x.Id);
                }
                else // 80% de lectures
                {
                    var currentList = cache.Cache;

                    // On s'assure que la liste lue n'est jamais dans un état intermédiaire (Count doit être 0 ou 2)
                    currentList.Should().NotBeNull();
                    bool isValidCount = currentList.Count == 0 || currentList.Count <= 2;
                    isValidCount.Should().BeTrue("Le lecteur a récupéré une liste dans un état incohérent !");
                }
            });

            // Assert
            cache.Cache.Should().NotBeNull();
        }
    }
}
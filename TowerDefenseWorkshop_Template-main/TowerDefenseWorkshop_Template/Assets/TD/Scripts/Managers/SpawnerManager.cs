namespace GSGD1
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	public enum SpawnerIndex
	{
		Spawner00,
		Spawner01,
		Spawner02,
	}

	public enum SpawnerStatus
	{
		Inactive = 0,
		WaveRunning
	}

	public class SpawnerManager : MonoBehaviour
	{
        [SerializeField]
		private List<EntitySpawner> _spawners = null;

		[SerializeField]
		private bool _autoStartNextWaves = false;

		[System.NonSerialized]
		private int _currentWaveSetIndex = -1;

		[System.NonSerialized]
		private int _currentWaveRunning = 0;

		[SerializeField]
		public UnityEvent<SpawnerManager, SpawnerStatus, int> WaveStatusChanged_UnityEvent = null;
        public UnityEvent WaveStatusStartedSpawning = null;
        public UnityEvent WaveStatusFinishedSpawning = null;
        public UnityEvent WaveStatusEnded = null;

        public delegate void SpawnerEvent(SpawnerManager sender, SpawnerStatus status, int runningWaveCount);
		public event SpawnerEvent WaveStatusChanged = null;

		[SerializeField]
		private int enemiesKilled = 0;
		[SerializeField]
		private int enemiesTeleported = 0;
		[System.NonSerialized]
		private int numberOfWaveSets;
		[System.NonSerialized]
		private int[] numberOfEnemiesPerWave;

        [SerializeField]
		private List<Damageable> _livingEntities = new List<Damageable>();

        //public delegate void EntityEvent(Damageable damageable);
        //public event EntityEvent EntityDied = null;

        [ContextMenu("Start waves")]
		public void StartWaves()
		{
			// Start a new wave set only if there are no currently a wave running
			if (_currentWaveRunning <= 0)
			{
				Debug.Log("Wave started!");
                WaveStatusStartedSpawning?.Invoke();
                StartNewWaveSet();
			}
		}

		private void Awake()
		{
            numberOfWaveSets = DatabaseManager.Instance.WaveDatabase.WaveSets.Count;
			numberOfEnemiesPerWave = new int[numberOfWaveSets];

            for (int i = 0; i < numberOfWaveSets; i++)
            {
                numberOfEnemiesPerWave[i] = 0;
                foreach (Wave wave in DatabaseManager.Instance.WaveDatabase.WaveSets[i].Waves)
                {
                    for (int x = 0; x < DatabaseManager.Instance.WaveDatabase.WaveSets[i].Waves.Count; x++)
                    {
                        foreach (WaveEntityDescription WED in DatabaseManager.Instance.WaveDatabase.WaveSets[i].Waves[x].WaveEntitiesDescription)
                        {
                            numberOfEnemiesPerWave[i] += 1;
                        }

                    }
                }

                numberOfEnemiesPerWave[i] = numberOfEnemiesPerWave[i] / DatabaseManager.Instance.WaveDatabase.WaveSets[i].Waves.Count;
            }

			for (int i = 0; i < numberOfEnemiesPerWave.Length; i++)
			{
                Debug.Log("WaveSet " + i + " has " + numberOfEnemiesPerWave[i] + " entities inside.");
            }
        }

		public void StartNewWaveSet()
		{
			_currentWaveSetIndex += 1;
			enemiesKilled = 0;
			enemiesTeleported = 0;
			var waveDatabase = DatabaseManager.Instance.WaveDatabase;

			if (waveDatabase.WaveSets.Count > _currentWaveSetIndex)
			{
				WaveSet waveSet = waveDatabase.WaveSets[_currentWaveSetIndex];
				List<Wave> waves = waveSet.Waves;

				for (int i = 0, length = _spawners.Count; i < length; i++)
				{
					if (i >= waves.Count)
					{
						Debug.LogWarningFormat("{0}.StartNewWaveSet() There are more spawner ({1}) than wave ({2}), discarding wave.", GetType().Name, _spawners.Count, waves.Count);
						break;
					}
					if (waves[i] == null)
					{
						Debug.LogWarningFormat("{0}.StartNewWaveSet() Null reference found in WaveSet at index {1}, ignoring.", GetType().Name, i);
						break;
					}
					_currentWaveRunning += 1;
					var spawner = _spawners[i];
					spawner.StartWave(waves[i]);
					spawner.WaveEnded.RemoveListener(Spawner_OnWaveStoppedSpawning);
					spawner.WaveEnded.AddListener(Spawner_OnWaveStoppedSpawning);

					WaveStatusChanged?.Invoke(this, SpawnerStatus.WaveRunning, _currentWaveRunning);
					WaveStatusChanged_UnityEvent?.Invoke(this, SpawnerStatus.WaveRunning, _currentWaveRunning);
				}
			}
			else
			{
                WaveStatusEnded.Invoke();
                Debug.Log("No waves left!");
				// No waves left : end game
			}
		}

		private void Spawner_OnWaveStoppedSpawning(EntitySpawner entitySpawner, Wave wave)
		{
			entitySpawner.WaveEnded.RemoveListener(Spawner_OnWaveStoppedSpawning);

			_currentWaveRunning -= 1;

			//WaveStatusChanged?.Invoke(this, SpawnerStatus.Inactive, _currentWaveRunning);
			//WaveStatusChanged_UnityEvent?.Invoke(this, SpawnerStatus.Inactive, _currentWaveRunning);

			if (_currentWaveRunning == 0)
			{
                WaveStatusFinishedSpawning?.Invoke();
            }

            // should we run a new wave?
            if (_autoStartNextWaves == true && _currentWaveRunning <= 0)
			{
				StartNewWaveSet();
			}
		}

		public void RegisterEntity(EntitySpawner entitySpawner, WaveEntity waveEntity)
		{
			//This function is linked to the EntitySpawner unityEvent.

			Damageable damageable = waveEntity.GetComponent<Damageable>();
			if (damageable != null)
			{
				_livingEntities.Add(damageable);
				damageable.DamageTaken += OnEntityDied;
            }
		}

		void OnEntityDied(Damageable caller, int currentHealth, int damageTaken)
		{
			if (currentHealth == 0)
			{
				enemiesKilled += 1;
				_livingEntities.Remove(caller);
			}

			if (enemiesKilled + enemiesTeleported == numberOfEnemiesPerWave[_currentWaveSetIndex])
			{
                WaveStatusChanged?.Invoke(this, SpawnerStatus.Inactive, 0);
                WaveStatusChanged_UnityEvent?.Invoke(this, SpawnerStatus.Inactive, 0);
                WaveStatusEnded.Invoke();
			}
		}

		public void OnEntityReachedPortal(WaveEntity waveEntity, Damageable caller)
		{
			enemiesTeleported += 1;
            _livingEntities.Remove(caller);
			Destroy(waveEntity.gameObject);

            if (enemiesKilled + enemiesTeleported == numberOfEnemiesPerWave[_currentWaveSetIndex])
            {
                WaveStatusChanged?.Invoke(this, SpawnerStatus.Inactive, 0);
                WaveStatusChanged_UnityEvent?.Invoke(this, SpawnerStatus.Inactive, 0);
                WaveStatusEnded.Invoke();
            }
        }
	}
}
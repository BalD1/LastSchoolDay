using UnityEngine;

public class CoinsOptimizer : Singleton<CoinsOptimizer>
{
    public const int MAX_AUDIO_COUNT = 10;
	private int currentAudioCount;

	public const int MAX_PARTICLES_COUNT = 30;
	private int currentParticlesCount;

	public void CreatedAudio(float audioLength)
	{
		currentAudioCount++;

		Invoke(nameof(AudioStopped), audioLength);
	}

	private void AudioStopped()
	{
		currentAudioCount--;
	}

	public bool CanPlayAudio() => currentAudioCount <= MAX_AUDIO_COUNT;

	public void CreatedParticles()
	{
		currentParticlesCount++;
	}

	public void ParticlesStopped()
	{ 
		currentParticlesCount--; 
	}

	public bool CanCreateParticles() => currentParticlesCount <= MAX_PARTICLES_COUNT;
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class CoinsOptimizer : MonoBehaviour
{
	private static CoinsOptimizer instance;
	public static CoinsOptimizer Instance
	{
		get
		{
			if (instance == null) Debug.LogError("CoinsOptimizer instance could not be found.");

			return instance;
		}
	}

    public const int MAX_AUDIO_COUNT = 10;
	private int currentAudioCount;

	public const int MAX_PARTICLES_COUNT = 10;
	private int currentParticlesCount;

    private void Awake()
	{
		instance = this;
	}

	public void CreatedAudio(float audioLength)
	{
		currentAudioCount++;

		Invoke(nameof(AudioStopped), audioLength);
	}

	private void AudioStopped()
	{
		currentAudioCount--;
	}

	public bool CanPlayAudio() => currentAudioCount < MAX_AUDIO_COUNT;

	public void CreatedParticles()
	{
		currentParticlesCount++;
	}

	public void ParticlesStopped()
	{ 
		currentParticlesCount--; 
	}

	public bool CanCreateParticles() => currentParticlesCount < MAX_PARTICLES_COUNT;
}
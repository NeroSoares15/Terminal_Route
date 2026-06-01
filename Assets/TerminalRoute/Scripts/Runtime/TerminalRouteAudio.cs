using TerminalRoute.Core;
using UnityEngine;

namespace TerminalRoute.Runtime
{
    public sealed class TerminalRouteAudio
    {
        private readonly AudioSource engineSource;
        private readonly AudioSource droneSource;
        private readonly AudioSource oneShotSource;
        private readonly AudioClip engineClip;
        private readonly AudioClip droneClip;

        public TerminalRouteAudio(GameObject owner)
        {
            var root = new GameObject("Terminal Route Audio");
            root.transform.SetParent(owner.transform, false);

            engineSource = root.AddComponent<AudioSource>();
            droneSource = root.AddComponent<AudioSource>();
            oneShotSource = root.AddComponent<AudioSource>();

            engineClip = CreateToneClip("TR Engine", 85f, 1.2f, 0.18f, true);
            droneClip = CreateToneClip("TR Drone", 42f, 2.5f, 0.12f, true);

            engineSource.clip = engineClip;
            engineSource.loop = true;
            engineSource.volume = 0.22f;

            droneSource.clip = droneClip;
            droneSource.loop = true;
            droneSource.volume = 0.18f;
        }

        public void PlayMenuDrone()
        {
            engineSource.Stop();
            if (!droneSource.isPlaying)
            {
                droneSource.Play();
            }
        }

        public void PlayRunLoop()
        {
            if (!engineSource.isPlaying)
            {
                engineSource.Play();
            }

            if (!droneSource.isPlaying)
            {
                droneSource.Play();
            }
        }

        public void UpdateRunAudio(GameState state, float speed, bool mirrorActive)
        {
            float sanityPressure = 1f - Mathf.Clamp01(state.Sanity / 100f);
            float speedPressure = Mathf.Clamp01(speed / 42f);

            engineSource.pitch = Mathf.Lerp(0.82f, 1.15f, speedPressure) + sanityPressure * 0.06f;
            engineSource.volume = Mathf.Lerp(0.10f, 0.24f, speedPressure);
            droneSource.pitch = mirrorActive ? 0.72f : Mathf.Lerp(0.88f, 1.06f, sanityPressure);
            droneSource.volume = mirrorActive ? 0.34f : Mathf.Lerp(0.14f, 0.28f, sanityPressure);
        }

        public void PlayStopChime()
        {
            oneShotSource.PlayOneShot(CreateToneClip("TR Stop", 320f, 0.18f, 0.17f, false), 0.70f);
            oneShotSource.PlayOneShot(CreateToneClip("TR Brake", 92f, 0.48f, 0.16f, true), 0.55f);
        }

        public void PlayStopApproach()
        {
            oneShotSource.PlayOneShot(CreateToneClip("TR Stop Approach", 410f, 0.16f, 0.12f, false), 0.50f);
        }

        public void PlayMissedStop()
        {
            oneShotSource.PlayOneShot(CreateToneClip("TR Missed Stop", 145f, 0.55f, 0.22f, true), 0.85f);
        }

        public void PlayDoorHiss()
        {
            oneShotSource.PlayOneShot(CreateNoiseClip("TR Door Hiss", 0.42f, 0.18f), 0.72f);
        }

        public void PlayLightsOut()
        {
            oneShotSource.PlayOneShot(CreateToneClip("TR Lights Out Drop", 52f, 0.82f, 0.25f, true), 0.95f);
            oneShotSource.PlayOneShot(CreateNoiseClip("TR Light Flicker", 0.62f, 0.12f), 0.65f);
        }

        public void PlayHazardHit()
        {
            oneShotSource.PlayOneShot(CreateToneClip("TR Hazard Hit", 74f, 0.24f, 0.25f, true), 0.78f);
            oneShotSource.PlayOneShot(CreateNoiseClip("TR Loose Object", 0.20f, 0.16f), 0.45f);
        }

        public void PlayEpisodePulse(EpisodeType episode)
        {
            float frequency = episode == EpisodeType.Monkey ? 110f : episode == EpisodeType.Ball ? 190f : 70f;
            oneShotSource.PlayOneShot(CreateToneClip("TR Episode", frequency, 0.45f, 0.18f, false), 0.9f);
        }

        public void PlayEndingTone()
        {
            engineSource.Stop();
            droneSource.Stop();
            oneShotSource.PlayOneShot(CreateToneClip("TR Ending", 58f, 1.1f, 0.25f, false), 0.95f);
        }

        private static AudioClip CreateToneClip(string name, float frequency, float duration, float amplitude, bool rough)
        {
            const int sampleRate = 22050;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            var data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float signal = Mathf.Sin(2f * Mathf.PI * frequency * t);
                if (rough)
                {
                    signal += Mathf.Sin(2f * Mathf.PI * (frequency * 0.53f) * t) * 0.45f;
                    signal += Mathf.Sin(2f * Mathf.PI * 7f * t) * 0.16f;
                }

                float envelope = Mathf.Clamp01(1f - (i / (float)samples) * (rough ? 0.12f : 1f));
                data[i] = signal * amplitude * envelope;
            }

            var clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static AudioClip CreateNoiseClip(string name, float duration, float amplitude)
        {
            const int sampleRate = 22050;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            var data = new float[samples];
            uint state = 0xA51CEu;

            for (int i = 0; i < samples; i++)
            {
                state = state * 1664525u + 1013904223u;
                float noise = ((state & 0xFFFFu) / 32768f) - 1f;
                float t = i / (float)samples;
                float envelope = Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI);
                data[i] = noise * amplitude * envelope;
            }

            var clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}

using TerminalRoute.Core;
using UnityEngine;

namespace TerminalRoute.Runtime
{
    public sealed class TerminalRouteAudio
    {
        private readonly AudioSource engineSource;
        private readonly AudioSource droneSource;
        private readonly AudioSource musicSource;
        private readonly AudioSource voiceSource;
        private readonly AudioSource oneShotSource;
        private readonly AudioClip engineClip;
        private readonly AudioClip droneClip;
        private readonly AudioClip menuMusicClip;
        private readonly AudioClip routeMusicClip;
        private readonly AudioClip nightmareMusicClip;
        private readonly AudioClip introVoiceClip;
        private readonly AudioClip nightmareIntroVoiceClip;
        private readonly AudioClip goodEndingVoiceClip;
        private readonly AudioClip lostEndingVoiceClip;
        private readonly AudioClip nightmareStartClip;
        private readonly AudioClip mirrorCheckClip;
        private readonly AudioClip stopChimeClip;
        private readonly AudioClip stopApproachClip;
        private readonly AudioClip missedStopClip;
        private readonly AudioClip doorHissClip;
        private readonly AudioClip doorDangerClip;
        private readonly AudioClip countConfirmClip;
        private readonly AudioClip countFailureClip;
        private readonly AudioClip dispatcherClip;
        private readonly AudioClip lightsOutClip;
        private readonly AudioClip hazardHitClip;
        private readonly AudioClip episodePulseClip;
        private readonly AudioClip endingToneClip;

        public TerminalRouteAudio(GameObject owner)
        {
            var root = new GameObject("Terminal Route Audio");
            root.transform.SetParent(owner.transform, false);

            engineSource = root.AddComponent<AudioSource>();
            droneSource = root.AddComponent<AudioSource>();
            musicSource = root.AddComponent<AudioSource>();
            voiceSource = root.AddComponent<AudioSource>();
            oneShotSource = root.AddComponent<AudioSource>();

            engineClip = LoadClip("TerminalRoute/Audio/SFX/EngineLoop") ?? CreateToneClip("TR Engine", 85f, 1.2f, 0.18f, true);
            droneClip = LoadClip("TerminalRoute/Audio/SFX/BusInterior") ?? CreateToneClip("TR Drone", 42f, 2.5f, 0.12f, true);
            menuMusicClip = LoadClip("TerminalRoute/Audio/Music/MenuDrone");
            routeMusicClip = LoadClip("TerminalRoute/Audio/Music/RouteLoop");
            nightmareMusicClip = LoadClip("TerminalRoute/Audio/Music/NightmareLoop");
            introVoiceClip = LoadClip("TerminalRoute/Audio/Voice/Intro");
            nightmareIntroVoiceClip = LoadClip("TerminalRoute/Audio/Voice/IntroNightmare");
            goodEndingVoiceClip = LoadClip("TerminalRoute/Audio/Voice/EndingGood");
            lostEndingVoiceClip = LoadClip("TerminalRoute/Audio/Voice/EndingLost");
            nightmareStartClip = LoadClip("TerminalRoute/Audio/SFX/NightmareStart");
            mirrorCheckClip = LoadClip("TerminalRoute/Audio/SFX/MirrorCheck");
            stopChimeClip = LoadClip("TerminalRoute/Audio/SFX/StopChime");
            stopApproachClip = LoadClip("TerminalRoute/Audio/SFX/StopApproach");
            missedStopClip = LoadClip("TerminalRoute/Audio/SFX/MissedStop");
            doorHissClip = LoadClip("TerminalRoute/Audio/SFX/DoorHiss");
            doorDangerClip = LoadClip("TerminalRoute/Audio/SFX/DoorDanger");
            countConfirmClip = LoadClip("TerminalRoute/Audio/SFX/CountConfirm");
            countFailureClip = LoadClip("TerminalRoute/Audio/SFX/CountFailure");
            dispatcherClip = LoadClip("TerminalRoute/Audio/SFX/Dispatcher");
            lightsOutClip = LoadClip("TerminalRoute/Audio/SFX/LightsOut");
            hazardHitClip = LoadClip("TerminalRoute/Audio/SFX/HazardHit");
            episodePulseClip = LoadClip("TerminalRoute/Audio/SFX/EpisodePulse");
            endingToneClip = LoadClip("TerminalRoute/Audio/SFX/EndingTone");

            engineSource.clip = engineClip;
            engineSource.loop = true;
            engineSource.volume = 0.22f;

            droneSource.clip = droneClip;
            droneSource.loop = true;
            droneSource.volume = 0.18f;

            musicSource.loop = true;
            musicSource.volume = 0.30f;

            voiceSource.loop = false;
            voiceSource.volume = 0.92f;
        }

        public void PlayMenuDrone()
        {
            engineSource.Stop();
            PlayMusic(menuMusicClip, 0.32f);
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
            EnsureRunMusic(state.IsNightmare);
            float sanityPressure = 1f - Mathf.Clamp01(state.Sanity / 100f);
            float speedPressure = Mathf.Clamp01(speed / 42f);
            float nightmarePitch = state.IsNightmare ? 0.09f : 0f;
            float nightmareVolume = state.IsNightmare ? 0.08f : 0f;

            engineSource.pitch = Mathf.Lerp(0.82f, 1.15f, speedPressure) + sanityPressure * 0.06f + nightmarePitch;
            engineSource.volume = Mathf.Clamp01(Mathf.Lerp(0.10f, 0.24f, speedPressure) + nightmareVolume * 0.45f);
            droneSource.pitch = mirrorActive ? 0.72f : Mathf.Lerp(0.88f, 1.06f, sanityPressure);
            droneSource.volume = mirrorActive ? 0.34f : Mathf.Clamp01(Mathf.Lerp(0.14f, 0.28f, sanityPressure) + nightmareVolume);
        }

        public void PlayNightmareStart()
        {
            if (PlayOptionalOneShot(nightmareStartClip, 0.92f))
            {
                return;
            }

            oneShotSource.PlayOneShot(CreateToneClip("TR Nightmare Start", 46f, 0.90f, 0.28f, true), 0.92f);
            oneShotSource.PlayOneShot(CreateNoiseClip("TR Nightmare Static", 0.55f, 0.20f), 0.62f);
        }

        public void PlayMirrorCheck()
        {
            if (PlayOptionalOneShot(mirrorCheckClip, 0.72f))
            {
                return;
            }

            oneShotSource.PlayOneShot(CreateToneClip("TR Mirror Check", 235f, 0.16f, 0.12f, true), 0.58f);
            oneShotSource.PlayOneShot(CreateToneClip("TR Mirror Check High", 470f, 0.10f, 0.08f, false), 0.36f);
        }

        public void PlayStopChime()
        {
            if (PlayOptionalOneShot(stopChimeClip, 0.74f))
            {
                return;
            }

            oneShotSource.PlayOneShot(CreateToneClip("TR Stop", 320f, 0.18f, 0.17f, false), 0.70f);
            oneShotSource.PlayOneShot(CreateToneClip("TR Brake", 92f, 0.48f, 0.16f, true), 0.55f);
        }

        public void PlayStopApproach()
        {
            if (PlayOptionalOneShot(stopApproachClip, 0.58f))
            {
                return;
            }

            oneShotSource.PlayOneShot(CreateToneClip("TR Stop Approach", 410f, 0.16f, 0.12f, false), 0.50f);
        }

        public void PlayMissedStop()
        {
            if (PlayOptionalOneShot(missedStopClip, 0.86f))
            {
                return;
            }

            oneShotSource.PlayOneShot(CreateToneClip("TR Missed Stop", 145f, 0.55f, 0.22f, true), 0.85f);
        }

        public void PlayDoorHiss()
        {
            if (PlayOptionalOneShot(doorHissClip, 0.74f))
            {
                return;
            }

            oneShotSource.PlayOneShot(CreateNoiseClip("TR Door Hiss", 0.42f, 0.18f), 0.72f);
        }

        public void PlayDoorDanger()
        {
            if (PlayOptionalOneShot(doorDangerClip, 0.88f))
            {
                return;
            }

            oneShotSource.PlayOneShot(CreateToneClip("TR Door Danger", 58f, 0.62f, 0.26f, true), 0.86f);
            oneShotSource.PlayOneShot(CreateNoiseClip("TR Door Wind", 0.50f, 0.22f), 0.58f);
        }

        public void PlayCountConfirm()
        {
            if (PlayOptionalOneShot(countConfirmClip, 0.58f))
            {
                return;
            }

            oneShotSource.PlayOneShot(CreateToneClip("TR Count Confirm", 520f, 0.12f, 0.14f, false), 0.55f);
            oneShotSource.PlayOneShot(CreateToneClip("TR Count Confirm Low", 260f, 0.16f, 0.10f, false), 0.38f);
        }

        public void PlayCountFailure()
        {
            if (PlayOptionalOneShot(countFailureClip, 0.82f))
            {
                return;
            }

            oneShotSource.PlayOneShot(CreateToneClip("TR Count Failure", 122f, 0.48f, 0.22f, true), 0.78f);
        }

        public void PlayDispatcher()
        {
            if (PlayOptionalOneShot(dispatcherClip, 0.54f))
            {
                return;
            }

            oneShotSource.PlayOneShot(CreateToneClip("TR Dispatcher", 180f, 0.20f, 0.11f, true), 0.44f);
        }

        public void PlayLightsOut()
        {
            if (PlayOptionalOneShot(lightsOutClip, 0.96f))
            {
                return;
            }

            oneShotSource.PlayOneShot(CreateToneClip("TR Lights Out Drop", 52f, 0.82f, 0.25f, true), 0.95f);
            oneShotSource.PlayOneShot(CreateNoiseClip("TR Light Flicker", 0.62f, 0.12f), 0.65f);
        }

        public void PlayHazardHit()
        {
            if (PlayOptionalOneShot(hazardHitClip, 0.82f))
            {
                return;
            }

            oneShotSource.PlayOneShot(CreateToneClip("TR Hazard Hit", 74f, 0.24f, 0.25f, true), 0.78f);
            oneShotSource.PlayOneShot(CreateNoiseClip("TR Loose Object", 0.20f, 0.16f), 0.45f);
        }

        public void PlayEpisodePulse(EpisodeType episode)
        {
            if (PlayOptionalOneShot(episodePulseClip, 0.86f))
            {
                return;
            }

            float frequency = episode == EpisodeType.Monkey ? 110f : episode == EpisodeType.Ball ? 190f : 70f;
            oneShotSource.PlayOneShot(CreateToneClip("TR Episode", frequency, 0.45f, 0.18f, false), 0.9f);
        }

        public float PlayIntroVoice(bool nightmare)
        {
            AudioClip clip = nightmare ? nightmareIntroVoiceClip != null ? nightmareIntroVoiceClip : introVoiceClip : introVoiceClip;
            PlayVoice(clip, 0.92f);
            return clip != null ? clip.length : 0f;
        }

        public void StopVoice()
        {
            voiceSource.Stop();
            voiceSource.clip = null;
        }

        public void PlayEndingTone()
        {
            PlayEndingTone(EndingId.LongRoute);
        }

        public void PlayEndingTone(EndingId ending)
        {
            engineSource.Stop();
            droneSource.Stop();
            musicSource.Stop();
            if (!PlayOptionalOneShot(endingToneClip, 0.95f))
            {
                oneShotSource.PlayOneShot(CreateToneClip("TR Ending", 58f, 1.1f, 0.25f, false), 0.95f);
            }

            PlayVoice(ending == EndingId.GoodTrip ? goodEndingVoiceClip : lostEndingVoiceClip, 0.94f);
        }

        private void EnsureRunMusic(bool nightmare)
        {
            AudioClip clip = nightmare && nightmareMusicClip != null ? nightmareMusicClip : routeMusicClip;
            PlayMusic(clip, nightmare ? 0.38f : 0.30f);
        }

        private void PlayMusic(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                return;
            }

            if (musicSource.clip == clip && musicSource.isPlaying)
            {
                musicSource.volume = volume;
                return;
            }

            musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.Play();
        }

        private void PlayVoice(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                return;
            }

            voiceSource.Stop();
            voiceSource.clip = clip;
            voiceSource.volume = volume;
            voiceSource.Play();
        }

        private bool PlayOptionalOneShot(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                return false;
            }

            oneShotSource.PlayOneShot(clip, volume);
            return true;
        }

        private static AudioClip LoadClip(string resourcePath)
        {
            return Resources.Load<AudioClip>(resourcePath);
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

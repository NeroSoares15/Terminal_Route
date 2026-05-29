using TerminalRoute.Core;
using UnityEngine;

namespace TerminalRoute.Runtime
{
    public sealed class MirrorThreatManager
    {
        public const float ReactionWindow = 1.15f;
        private const float WarmupSeconds = 6f;
        private const float SpawnChance = 0.55f;
        private const float CooldownSeconds = 9f;

        private readonly MirrorView mirror;
        private readonly GameObject threatVisual;
        private readonly TerminalRouteAudio audio;
        private float runTime;
        private float cooldown;
        private float threatTime;
        private bool threatActive;
        private bool firstEligibleThreatUsed;

        public MirrorThreatManager(MirrorView mirror, GameObject threatVisual, TerminalRouteAudio audio)
        {
            this.mirror = mirror;
            this.threatVisual = threatVisual;
            this.audio = audio;
            mirror.Activated += OnMirrorActivated;
            mirror.Deactivated += ClearThreat;
        }

        public bool IsThreatActive
        {
            get { return threatActive; }
        }

        public string WarningText
        {
            get { return threatActive ? "OLHA PARA A FRENTE" : ""; }
        }

        public void Reset()
        {
            runTime = 0f;
            cooldown = 6f;
            threatTime = 0f;
            firstEligibleThreatUsed = false;
            ClearThreat();
        }

        public void Tick(float deltaTime, GameState state)
        {
            if (state.Phase == GamePhase.Ended)
            {
                return;
            }

            runTime += deltaTime;
            cooldown = Mathf.Max(0f, cooldown - deltaTime);

            if (!threatActive || !mirror.IsActive)
            {
                return;
            }

            threatTime += deltaTime;
            if (ReactionExpired(threatTime))
            {
                state.ResolveEnding(EndingId.LongRoute, EndingCause.CloseNpcStare);
            }
        }

        public static bool ReactionExpired(float visibleSeconds)
        {
            return visibleSeconds >= ReactionWindow;
        }

        private void OnMirrorActivated()
        {
            if (runTime < WarmupSeconds || cooldown > 0f || threatActive)
            {
                return;
            }

            if (firstEligibleThreatUsed && Random.value > SpawnChance)
            {
                return;
            }

            firstEligibleThreatUsed = true;
            threatActive = true;
            threatTime = 0f;
            if (threatVisual != null)
            {
                threatVisual.SetActive(true);
            }

            if (audio != null)
            {
                audio.PlayEpisodePulse(EpisodeType.Silence);
            }
        }

        private void ClearThreat()
        {
            if (threatActive)
            {
                cooldown = CooldownSeconds;
            }

            threatActive = false;
            threatTime = 0f;
            if (threatVisual != null)
            {
                threatVisual.SetActive(false);
            }
        }
    }
}

using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.Relic;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.EnemyBehaviour.EnemyActions
{
    public class EnemyAttackAction : EnemyActionBase
    {
        public override EnemyActionType ActionType => EnemyActionType.Attack;

        public override void DoAction(EnemyActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;

            var value = Mathf.Max(0, Mathf.RoundToInt(actionParameters.Value +
                actionParameters.SelfCharacter.CharacterStats.StrengthModifier));

            if (actionParameters.SelfCharacter.CharacterStats.StatusDict[StatusType.Weak].StatusValue > 0)
                value = Mathf.RoundToInt(value * 0.75f);

            if (actionParameters.TargetCharacter.CharacterStats.StatusDict[StatusType.Vulnerable].StatusValue > 0)
                value = Mathf.RoundToInt(value * 1.5f);

            actionParameters.TargetCharacter.CharacterStats.Damage(value);
            if (actionParameters.TargetCharacter is AllyBase)
                RelicEffectProcessor.Trigger(RelicCondition.OnDamageTaken,
                    new RelicEffectParameters
                    {
                        Value = value,
                        SourceCharacter = actionParameters.SelfCharacter,
                        TargetCharacter = actionParameters.TargetCharacter
                    });

            if (FxManager != null)
            {
                FxManager.PlayFx(actionParameters.TargetCharacter.transform, FxType.Attack);
                FxManager.SpawnFloatingText(actionParameters.TargetCharacter.TextSpawnRoot, value.ToString());
            }

            if (AudioManager != null)
                AudioManager.PlayOneShot(AudioActionType.Attack);
        }
    }
}

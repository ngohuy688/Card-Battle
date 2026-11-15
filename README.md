# Nue Deck - Roguelike Card Game Template
Origin Repository: https://github.com/Arefnue/NueDeck

## Relics

Create relic assets from **NueDeck > Collection > Relic** and assign a unique ID, icon, description, effect, trigger condition, and effect value. Add a `RelicRewardData` asset to the `Relic Reward Data List` on the `RewardContainerData` asset to make its relics available as choices. Relic choices are currently offered after a stage boss victory.

The sample `Training Fang` relic adds its configured damage bonus to each ally attack. Implement additional effects by deriving from `RelicEffectBase` and adding a matching value to `RelicEffectType`. Owned relics persist for the current run and duplicate relic IDs are rejected.

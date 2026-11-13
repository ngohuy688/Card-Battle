# Nue Deck - Roguelike Card Game Template

- Create your own single player roguelike deck-building card game with a few easy steps.

- You can go fast with ready to use mechanics and assets, or you can add your own mechanics and assets easily.

- This template is suitable for new learners, game designers, artists and game jammers. It can also provide a good starting point to develop bigger projects, so experienced developers also can use this template as a basis.

#
[**Documentation**](https://docs.google.com/document/d/1CRl4W29OeiZ2gzUf0JWgWdSO5soBu5oYF-Qzl9uxppQ/edit#heading=h.vi8cvdabxzzc)

[**Discord**](https://discord.com/invite/5FkXvHChUr)

[**Web Demo**](https://arefnue.itch.io/nue-deck)

[**AssetStore**](https://assetstore.unity.com/packages/templates/systems/nue-deck-roguelike-card-game-template-221223)

# Contributing

If you want to contribute this project with your scripts or assets, you can create merge request on Github repository or reach me via Discord, E-mail or Linkedin. 

# Features
- Easy to use card editor and create new contents
- Scriptable object based architecture
- Ready to use basic assets
- Open source

# Spine character animations

The character animation view is optional. To use it, add a `SkeletonAnimation` and
`CharacterSpineView` to the character prefab (the view can be on the same GameObject
or a parent/child), assign the Spine skeleton, and set the animation names in the
view. Leave a name empty when that animation is not available on the skeleton.

The view uses non-looping attack, hit, and heal clips, then queues idle; idle loops.
Death is non-looping and the character is destroyed when the clip completes. If no
view or death clip is configured, the existing character behavior is retained.
Combat state and damage resolution remain in the character/card/enemy systems.

# Character selection

The Main Menu start button opens `SelectionCanvas`. Add the canvas under the
persistent `UIManager` prefab, attach `SelectionCanvas` to its root, and assign
that component to the `UIManager.SelectionCanvas` field. Assign the left, right,
and select buttons, plus the portrait `Image` components for the left, selected,
and right positions. The select button's TMP label is optional.

Add the available ally prefabs to `AllyRoster` on the `GameplayData` asset. The
selected ally becomes the only active party member and confirmation continues to
the Map scene. Optionally assign a portrait sprite in each ally's
`AllyCharacterData`; if omitted, the selection UI uses the first sprite renderer
found in that ally prefab.

# Branching encounter maps

`Encounter Data` defines the map graph for each stage. Set `Map Start Node Id`,
then add `Map Node List` entries with unique node IDs, display names, node type,
encounter index, room number, normalized canvas position, and next-node IDs.
Multiple nodes can point to the same next node to create a convergence; a node
can point to multiple successors to create a choice. Boss nodes select a specific
entry from that stage's boss encounter list, so separate boss nodes can lead to
different boss encounters.

The `MapManager` prefab must reference the `EncounterButton` prefab. The manager
builds the visible nodes and connecting lines from the current stage graph at
runtime, so the static buttons under `MapCanvas/EncounterParent` are cleared.
Only the starting node or successors of the last completed node are selectable.
After a regular encounter, its successors are unlocked; winning any boss node
finishes the stage. A boss in a stage with a configured successor stage grants
the standard Gold and Card rewards before the reward screen advances to the next
stage; bosses in the last configured stage retain the game-ending win panel.

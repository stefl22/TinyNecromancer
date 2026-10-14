# Reference protagonist: initial editable study

Status: appearance study, NOT a final likeness or production-ready character. No replacement approved.

## Files

- `ReferenceHero_v03.blend`: editable geometry and packed user-supplied reference image.
- `Reference.png`: primary reference, supplied by the user; not a downloaded mesh.
- `build_reference.py`: reproducible Blender construction, reuses the preserved v02 anatomical surface helpers.
- Unity FBX: `Assets/_Game/Art/ReferenceHero_v03/ReferenceHero_v03.fbx`.
- Review scene: `Assets/_Game/Scenes/ReferenceHero_Review_v03.unity`.
- Actual Unity capture: `ArtReview/ReferenceHero_v03_Unity.png`.

Scene shows front, side and back copies under studio lighting. Select a model in Scene view and press F to inspect and orbit. Game view shows all three without needing Play Mode. The playable scene and its character are unchanged.

## What exists

Custom mesh surfaces with editable vertices: draped burgundy scarf/hood, long divided tabard, dark doublet, linen sleeves, leather bracers, gloves, boots, belt/bag, asymmetric shoulder plates with brass borders, layered closed-volume hair locks and sheathed sword. Approximately 9,807 source vertices in 271 individually editable pieces before export modifiers. Generated linen/leather/metal texture maps and Unity URP materials. This object count is for editing and must be consolidated before gameplay use.

## Visual findings and limitations

Checked actual FBX rendering in Unity from front, profile and back. Corrected exposed scalp and exaggerated shoulder peaks. Active Unity Console reports no errors/warnings and compilationFailed=false; this is import validation, NOT evidence of artistic quality.

The face does not yet closely reproduce the reference: eye/eyelid construction, cheek/jaw planes, ears and expression need direct sculpting. Hair is too angular and mechanically repetitive. Garment folds, armor curvature, boot silhouette and surface wear remain simplified. Some layered surfaces need further intersection cleanup. This is an initial study, not fulfillment of the commercial quality target.

No armature, skin weights, animations, facial rig, LODs or validated deformation topology. No animation or performance quality claim. Do not attach it to the playable controller yet.

## Feasible free workflow on this machine

Observed hardware: NVIDIA RTX 5050 laptop GPU, 8,151 MiB VRAM; approximately 16 GB system RAM. Blender 5.2.1 LTS is installed. No new generator or dependencies were installed.

TripoSR official repository https://github.com/VAST-AI-Research/TripoSR documents approximately 6 GB VRAM for default single-image inference and MIT licensing for code and pretrained models. It is a plausible local experiment, requiring a compatible Python/PyTorch/CUDA environment and compiled dependencies. Compatibility and output quality have not been tested on this machine. A clean isolated single-character view would be needed; this entire multi-panel sheet should not be passed as a single object view.

Hunyuan3D 2.1 https://github.com/Tencent-Hunyuan/Hunyuan3D-2.1 documents 10 GB shape, 21 GB texture, 29 GB combined VRAM. Its stock configuration exceeds this GPU's memory. License: https://github.com/Tencent-Hunyuan/Hunyuan3D-2.1/blob/main/LICENSE (custom community terms, not MIT). Do not install without reviewing intended use and all dependencies.

Recommended current workflow: use supplied front/profile/back as visual guides (they are perspective illustrations, not exact orthographic blueprints); sculpt facial landmarks and anatomical volumes; model separate garments and equipment; author directional hair clumps; manually retopologize face/joints/hands; unwrap UVs and bake high-detail surfaces into normals/material maps; review likeness in Unity; then humanoid rig, weights and motion/deformation tests. Automatic output could be a sculpt reference but is not a substitute for these steps.

## Preservation

Previous teal study checkpoint: 3f1e775. Original playable assets, scenes, original Blender files and all gameplay scripts retained. This review is on codex/combat-stability in a separate asset namespace.

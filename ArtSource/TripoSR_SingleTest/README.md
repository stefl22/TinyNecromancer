# Single TripoSR trial — rejected for gameplay replacement

Exactly one TripoSR neural reconstruction was performed on the frontal panel of the user-supplied character reference. No second variant, training, cloud generation or background server was used. Original playable assets were not modified. This result is archived for inspection only.

## Result

The mesh follows the broad clothing arrangement more closely than the procedural study, but its face, hair and hands are deformed; accessories and cloth details merge into soft, irregular surfaces. It does not clearly exceed the existing character's overall quality. The experiment was stopped rather than continued or integrated into gameplay.

27,646 vertices, 55,288 triangles, vertex colors, no rig or skinning. The single inference plus extraction took approximately 13 seconds after loading; peak PyTorch GPU allocation was approximately 2,379 MiB. Installation and first loading took substantially longer. Metrics are not evidence of visual quality.

## Retained final files

- `TripoSR_Character.blend`: editable mesh, vertex-color material and review cameras/lights; upright and scaled to 1.85 m.
- `TripoSR_Character.fbx`: upright mesh export with vertex colors; a Unity shader that reads vertex colors is required to display those colors. Not installed in Assets or the playable scene.
- `TripoSR_Character.glb`: original colored reconstruction, before Blender orientation/scale normalization.
- `Front.png`, `Side.png`, `Back.png`: rendered views of that same mesh, not generated concept images.
- `generation.json`: generation measurements.
- `cleanup.json`: cleanup and baseline-comparison verification.
- This report.

The supplied reference already remains in `ArtSource/ReferenceHero_v03/Reference.png`; no duplicate reference or preprocessing images are retained here.

## Method and provenance

Official TripoSR source: https://github.com/VAST-AI-Research/TripoSR
Source revision: `107cefdc244c39106fa830359024f6a2f1c78871`.
Official weights: `stabilityai/TripoSR`, snapshot `5b521936b01fbe1890f6f9baed0254ab6351c04a`.
The official repository states MIT licensing for source and pretrained models.

Python 3.12 isolated venv, PyTorch 2.7.1+cu128 on RTX 5050 laptop 8 GB. Background removal used local U2Net. CUDA compatibility was verified before inference. Transformers 4.38.2 was used for Python 3.12 compatibility. The neural architecture/weights were not altered. CPU PyMCubes triangulated the inferred scalar field instead of compiling torchmcubes; coordinate ordering was adapted to its API. Extraction resolution 256; renderer query chunk size 4096. Only the front reference panel was used, not the whole multi-panel sheet.

HTTPS certificate verification remained enabled; truststore used Windows trusted certificates. No system Python packages, CUDA toolkit, services, scheduled tasks or Windows startup entries were installed. Existing Blender and Codex's existing Python runtime were reused and retained.

## Cleanup scope

The disposable installation directory was `C:\TINY_NECROMANCER\TinyNecromancer\Temp\TripoSR_OneTest`. It contained the clone, venv and all added Python dependencies, TripoSR/U2Net weights, Hugging Face/PyTorch/pip/Numba/CUDA caches, temporary preprocessing/export scripts and baseline snapshots. Its measured size before deletion was 8,508,041,073 bytes (about 8.51 GB decimal).

All contents of that directory were removed after verifying final assets and that the test processes had exited. Redundant intermediate PLY, preprocessing images and this test's intermediate `.blend1` were also removed. No pre-existing Blender source or backup was removed.

The service list/start modes, scheduled task identities and current-user Run entries matched the pre-installation baseline. See `cleanup.json` for verified deletion and final process/GPU checks. No claim is made that total machine GPU usage is zero: Unity and Windows can continue using GPU memory independently of this removed test.

# 🏷️ GLTFAttributesUserTextExporter for Rhino　(beta)

GLTFAttributeExporter is a plugin for Rhinoceros that allows you to export your models in gltf/glb format while including attributes user text assigned to the Geometry.

## :information_source: Features
### :satisfied: **Support**
- 🎨 **Export models from Rhinoceros in gltf/glb format.**
- 🏷️ **Include attributes user text assigned to the Geometry in the export.**
- 🗂️ **Choose to export the models with layers as separate nodes in the glTF file, or export all models in a flat array without layer separation.**

### :confounded: **Not Support**
- :x: **Draco compression.**
- :x: **Export Point Cloud.**
- :x: **Export VertexColor.**

### :sunglasses: **Note** 
- If you want to apply Draco compression to the model exported using this plugin, please use [gltf-pipeline](https://github.com/CesiumGS/gltf-pipeline). After reviewing various tools, I found that some of them may cause attribute user text loss when applying Draco compression. It has been confirmed that gltf-pipeline can apply Draco compression while retaining attributes user text. I have created a sample code for applying Draco compression using gltf-pipeline, so if necessary, please refer to the repository below.

   => [Sample Code Draco Compression using gltf-pipeline](https://github.com/shuya-tamaru/gltf-draco-compression) 🚀



## :arrow_down_small: Installation

1. **Download**:
   - Visit the [releases page](https://github.com/shuya-tamaru/GltfAttributesUserTextExporter/releases) of this repository.
   - Download the latest release file named `v1.0.0 GltfAttributesUserTextExporter.zip`.

2. **Extract**:
   - Unzip the downloaded `v1.0.0 GltfAttributesUserTextExporter.zip` file.

3. **Copy to Plugins Folder**:
   - Copy the extracted `unziped　folder` to the Rhinoceros plugins folder.
     - For Rhino 7: `C:\Users\<YourUsername>\AppData\Roaming\McNeel\Rhinoceros\7.0\Plug-ins\`
     - For Rhino 8: `C:\Users\<YourUsername>\AppData\Roaming\McNeel\Rhinoceros\8.0\Plug-ins\`

4. **Install in Rhinoceros**:
   - Open Rhinoceros.
   - Go to `Tools` -> `Options` -> `Plugins`.
   - Click on `Install` at the bottom of the window.
   - Browse to the `GLTFAttributesUserTextExporter.rhp` file in the plugins folder and select it.

5. **Enable the Plugin**:
   - Ensure that `GLTFAttributesUserTextExporter` is listed and enabled in the installed plugins list. If it is not enabled, check the box to enable it.

6. **Usage**:
   - Load the model you want to export in Rhinoceros.
   - Type `GltfAttributesExport` in the command line and press Enter.
   - Your model will be exported in gltf/glb format with all attributes user text included.


## :fast_forward: Quick Start
 🎥 Watch the Quick Start Video
 
 [![Watch the video](https://img.youtube.com/vi/QARcmx5jKZk/maxresdefault.jpg)](https://www.youtube.com/watch?v=QARcmx5jKZk)
 
## :arrows_clockwise: Compatibility

The plugin has been tested and confirmed to work with:

- 🖥️ **Windows Rhino 7**
- 🖥️ **Windows Rhino 8**

## :record_button: Usage

1. Install the plugin from food4Rhino.
2. Open Rhinoceros and load your model.
3. In Rhinoceros, enter the command `GltfAttributesExport`.
4. Your model will be exported in gltf/glb format with all user text attributes included.

## 🎦 Viewing Exported Models
You can check the exported models using the following viewers:

[![](https://img.shields.io/badge/-Three.js-ffffff.svg?logo=threedotjs&logoColor=000000)](https://threejs.org/)
[![](https://img.shields.io/badge/-ReactThreeFiber-444444.svg?logo=react)](https://docs.pmnd.rs/react-three-fiber/getting-started/introduction)
[![](https://img.shields.io/badge/-Babylon.js-DC3D24.svg?logo=Babylon)](https://doc.babylonjs.com/)

 ### 1. Three.js (React Three Fiber) Viewer
- Each Mesh's `UserData` contains `gltf/glb` extras, which are the attribute user text assigned to each geometry in Rhinoceros.
- If you want to perform a walkthrough inside the model (for example, if the model is a building), please set the UserText for the geometry you want to walk on in Rhinoceros with key = "isWalking" and value = "true".
  
  => [Three.js (React Three Fiber) Viewer](https://gltf.styublog.com/three-viewer) 🚀
### 2. Babylon.js Viewer
- Each Mesh's `Metadata` contains `gltf/glb` extras, which are the attribute user text assigned to each geometry in Rhinoceros.
  
  => [Babylon.js Viewer](https://gltf.styublog.com/babylon-viewer) 🚀

<div align="center">

 ### :bulb:For All Viewers You can use the `developer tools` to check the loaded model in the console for all viewers.
</div>

## :record_button: Documentation

  => For more detailed information and advanced usage, please visit the [official documentation.](https://gltf.styublog.com)
  The content is very similar to this README.


## :arrow_down_small: Contact

For any questions or support, please open an issue on GitHub 

or contact at ↓

[![X](https://img.shields.io/badge/Follow_@tama20013_-shuya_tamaru-0000FF.svg?style=flat-square&logo=x&logoColor=white)](https://twitter.com/tama20013)

## :record_button: License
This project is licensed under the MIT License. For more details, please see the [LICENSE file](https://github.com/shuya-tamaru/GltfAttributesUserTextExporter?tab=License-1-ov-file).

---

<div align="center">

## :tada: Enjoy using GLTFAttributesUserTextExporter ! :tada:
</div>


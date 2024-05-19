# 🏷️ GLTFAttributesUserTextExporter for Rhino

GLTFAttributeExporter is a plugin for Rhinoceros that allows you to export your models in gltf/glb format while including attributes user text assigned to the Geometry.

## :information_source: Features
### :satisfied: **Support**
- 🎨 **Export models from Rhinoceros in gltf/glb format.**
- 🏷️ **Include attributes user text assigned to the Geometry in the export.**
- 🖼️ **Export textures along with the models.**
- 🗂️ **Choose to export the models with layers as separate nodes in the glTF file, or export all models in a flat array without layer separation.**

### :confounded: **Not Support**
- :x: **Draco compression.**
- :x: **Export Point Cloud.**
- :x: **Export VertexColor.**

### :sunglasses: **Note** 
- If you want to apply Draco compression to the model exported using this plugin, please use [gltf-pipeline](https://github.com/CesiumGS/gltf-pipeline). After reviewing various tools, I found that some of them may cause attribute user text loss when applying Draco compression. It has been confirmed that gltf-pipeline can apply Draco compression while retaining attributes user text. I have created a sample code for applying Draco compression using gltf-pipeline, so if necessary, please refer to the repository below.

   => [Sample Code Draco Compression using gltf-pipeline](https://github.com/shuya-tamaru/gltf-draco-compression) 🚀



## :arrow_down_small: Installation

- :rhinoceros: You can install the plugin from  [food4Rhino](https://www.food4rhino.com) .

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
[![](https://img.shields.io/badge/-PlayCanvas-182326.svg?logo=playcanvas)](https://playcanvas.com/)  

 ### :one: Three.js (React Three Fiber) Viewer
- Each Mesh's `UserData` contains `gltf/glb` extras, which are the attribute user text assigned to each geometry in Rhinoceros.
  
  => [Three.js (React Three Fiber) Viewer](#) 🚀
### :two: Babylon.js Viewer
- Each Mesh's `Metadata` contains `gltf/glb` extras, which are the attribute user text assigned to each geometry in Rhinoceros.
  
  => [Babylon.js Viewer](#) 🚀

### :three: PlayCanvas Viewer
- Coming soon...
  
  => [PlayCanvas Viewer](#) 🚀


<div align="center">

 ### :bulb:For All Viewers You can use the `developer tools` to check the loaded model in the console for all viewers.
</div>


## :arrow_down: Contact

For any questions or support, please open an issue on GitHub 

or contact at ↓

[![X](https://img.shields.io/badge/Follow_@tama20013_-shuya_tamaru-0000FF.svg?style=flat-square&logo=x&logoColor=white)](https://twitter.com/tama20013)

---

<div align="center">

## :sun_with_face: Enjoy using GLTFAttributesUserTextExporter ! :full_moon_with_face:
</div>


# lathe_system
This Unity script, titled "DeformacionTorno," is designed to facilitate dynamic deformation of a 3D object in response to collisions. The primary functionality involves modifying the vertices of the mesh when a collision occurs, resulting in a realistic deformation effect that simulates a lathe machine.

## Features

- **Vertex Neighbor Storage**: The script efficiently stores the neighbors of each vertex in a mesh, providing a foundation for subsequent calculations.
  
- **Vertex Triangle Storage**: Triangle indices corresponding to each vertex are stored, aiding in the identification of affected vertices during collisions.

- **Vertex Duplicates Handling**: The script manages duplicate vertices and their respective lists, ensuring accurate vertex manipulation during collisions.

- **Collision Detection and Response**: Utilizing Unity's OnCollisionEnter method, the script identifies collision points, transforms them into local space, and determines the nearest vertex. The corresponding vertices are then offset based on the collision, creating a deformation effect.

- **User-Defined Deformation Axis**: The script allows users to specify the axis (X, Y, or Z) along which the deformation occurs, providing flexibility in achieving desired effects.

- **Adjustable Deformation Parameters**: Users can fine-tune deformation parameters such as valorResta, precisionColision, and ejeHorizontal to achieve the desired deformation behavior.

## Usage

1. Attach the script to a GameObject with a MeshFilter component.
2. Ensure the GameObject has a Rigidbody and MeshCollider component, as specified by [RequireComponent] attributes in the script.
3. Adjust parameters such as valorResta, precisionColision, and ejeHorizontal to achieve the desired deformation behavior.
4. Collisions with the assigned Collider trigger the deformation effect.

## Additional Notes

- The script dynamically recalculates vertex information during runtime to maintain accuracy.
- Deformation is based on the nearest vertex to the collision point and affects its duplicates.
- The script includes methods for handling vertex offset, collision detection, and normalization.
- Deformed mesh updates its normals and bounds for accurate rendering and collision detection.

Feel free to explore and modify this script to suit your specific project requirements. If you encounter any issues or have questions, please refer to the accompanying documentation or contact the script author.

## Author

Txomin Errasti  

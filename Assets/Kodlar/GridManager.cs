using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public GameObject cubePrefab;

    public List<Vector3> placedCubes = new List<Vector3>();

    public void AddCube(Vector3 pos)
    {

        Vector3 cleanPos = new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));

  
        if (!placedCubes.Contains(cleanPos))
        {
            placedCubes.Add(cleanPos);
            Instantiate(cubePrefab, cleanPos, Quaternion.identity);
            
            Debug.Log($"Küp Sayısı: {placedCubes.Count} | Konum: {cleanPos}");
        }
    }
    
    public void RemoveCube(Vector3 position)
    {
        if (placedCubes.Contains(position))
        {
            placedCubes.Remove(position);
        
            // O koordinatta duran fiziksel küpü sahnede bul ve yok et
            GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
            foreach (GameObject cube in cubes)
            {
                // Küpün pozisyonu ile silinmek istenen koordinat eşleşiyor mu?
                if (Vector3.Distance(cube.transform.position, position) < 0.1f)
                {
                    Destroy(cube);
                    break;
                }
            }
        }
    }
}
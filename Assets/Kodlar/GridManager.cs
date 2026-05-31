using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Blok Çeşitleri")]
    public GameObject[] blockPrefabs; // Plank, Oak, Cobblestone, Brick prefablarını buraya sürükle

    // YENİ: Artık sadece pozisyon değil, hangi pozisyonda hangi bloğun (0,1,2,3) olduğunu tutuyoruz
    public Dictionary<Vector3, int> placedBlocks = new Dictionary<Vector3, int>();

    public void AddCube(Vector3 pos, int blockIndex)
    {
        Vector3 cleanPos = new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));

        // Eğer o noktada zaten bir blok yoksa
        if (!placedBlocks.ContainsKey(cleanPos))
        {
            placedBlocks.Add(cleanPos, blockIndex); // Pozisyonu ve blok tipini kaydet
            
            GameObject yeniKup = Instantiate(blockPrefabs[blockIndex], cleanPos, Quaternion.identity);
            yeniKup.tag = "Cube"; 
            
            Debug.Log($"Blok Eklendi. Tip: {blockIndex} | Konum: {cleanPos}");
        }
    }
    
    public void RemoveCube(Vector3 position)
    {
        Vector3 cleanPos = new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
        
        if (placedBlocks.ContainsKey(cleanPos))
        {
            placedBlocks.Remove(cleanPos); // Listeden sil
        
            GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
            foreach (GameObject cube in cubes)
            {
                if (Vector3.Distance(cube.transform.position, cleanPos) < 0.1f)
                {
                    Destroy(cube);
                    break;
                }
            }
        }
    }
}
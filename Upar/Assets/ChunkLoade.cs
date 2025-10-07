using UnityEngine;
using System.Collections.Generic;

public class SectionManager : MonoBehaviour
{
    public Transform player;
    public float sectionLength = 50f; // Largo de cada sección en Z
    public int keepSections = 2;      // Cuántas secciones mantener activas (ej: actual + 1 adelante)

    public List<GameObject> sections = new List<GameObject>(); // Tus secciones ordenadas en lista

    private int currentIndex = 0;

    void Update()
    {
        if (player == null || sections.Count == 0) return;

        // Calculamos en qué sección está el jugador (en base a Z)
        int index = Mathf.FloorToInt(player.position.z / sectionLength);

        if (index != currentIndex)
        {
            currentIndex = Mathf.Clamp(index, 0, sections.Count - 1);
            UpdateSections();
        }
    }

    void UpdateSections()
    {
        for (int i = 0; i < sections.Count; i++)
        {
            if (Mathf.Abs(i - currentIndex) <= keepSections)
                sections[i].SetActive(true);   // Activar secciones cercanas
            else
                sections[i].SetActive(false);  // Desactivar secciones lejanas
        }
    }
}

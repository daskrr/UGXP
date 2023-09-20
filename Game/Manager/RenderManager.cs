using UGXP.Core;
using UGXP.Core.Components;
using UGXP.Tools;

namespace UGXP.Game.Manager;

internal class RenderManager
{
    private List<List<GameObject>> renderers;

    public RenderManager() {
        renderers = new();

        for (int i = 0; i < LayerManager.sortingLayers.Count; i++)
            renderers.Add(new List<GameObject>());
    }

    /// <summary>
    /// Tries to add any game object to the renderable list of objects. If the object does not meet criteria, it will not be rendered/added.
    /// </summary>
    /// <param name="obj">The object to render</param>
    public void Add(GameObject obj) {
        if (!obj || !obj.active) return;
        if (obj.renderer == null || !obj.renderer.active) return;

        int sortingLayer = obj.renderer.sortingLayer;
        if (sortingLayer >= renderers.Count) return; // this cannot happen

        if (renderers[sortingLayer].Contains(obj)) return;

        renderers[sortingLayer].Add(obj);
        SortRenderers(sortingLayer);
    }

    /// <summary>
    /// Updates a game object to try to add it as a renderer or removes it if it became [destroyed | !active]
    /// </summary>
    /// <param name="obj">The object to update</param>
    public void Update(GameObject obj) {
        if (!obj || !obj.active || obj.renderer == null || !obj.renderer.active) { 
            Remove(obj);
            return;
        }

        int sortingLayer = obj.renderer.sortingLayer;
        if (sortingLayer >= renderers.Count) return; // this cannot happen

        if (renderers[sortingLayer].Contains(obj)) return;

        renderers[sortingLayer].Add(obj);
        SortRenderers(sortingLayer);
    }

    /// <summary>
    /// Removes an object from the renderable objects list
    /// </summary>
    /// <param name="obj">The object to remove</param>
    public void Remove(GameObject obj) {
        for (int i = 0; i < renderers.Count; i++) {
            if (renderers[i].Contains(obj)) { 
                renderers[i].Remove(obj);
                SortRenderers(i);
                return;
            }
        }
    }

    private class RendererComparer : IComparer<GameObject> {
        public int Compare(GameObject? x, GameObject? y) {
            if (x != null && y == null) return 1;
            if (x == null && y == null) return 0;
            if (x == null && y != null) return -1;

            if (x == null || y == null) return 0;

            Renderer xR = x.renderer;
            Renderer yR = y.renderer;

            if (xR.sortingLayer > yR.sortingLayer)
                return 1;

            if (xR.sortingLayer == yR.sortingLayer) {
                if (xR.sortingOrder > yR.sortingOrder)
                    return 1;
                if (xR.sortingOrder < yR.sortingOrder)
                    return -1;

                return 0;
            }

            if (xR.sortingLayer < yR.sortingLayer)
                return -1;

            return 0;
        }
    }

    /// <summary>
    /// Sorts the renderable objects based on their sorting layer and sorting order in layer to be rendered properly on top of one another
    /// </summary>
    private void SortRenderers(int sortingLayer) {
        renderers[sortingLayer].Sort(new RendererComparer());
    }

    /// <summary>
    /// Renders all objects in the active scenes based on their sorting layers and order in those layers.
    /// </summary>
    public void Render() {
        renderers.ForEach(layer => layer.ForEach(obj => obj.renderer.Render()));
        Gizmos.Render();
    }
}

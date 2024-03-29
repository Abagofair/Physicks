﻿using GameUtilities.Scene;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUtilities.Rendering;

namespace MonoGameUtilities;

public static class SceneGraphExtensions
{
    public static void SetupBuffers(this SceneGraph sceneGraph, GraphicsDevice graphicsDevice)
    {
        if (sceneGraph == null) throw new ArgumentNullException(nameof(sceneGraph));

        foreach (RenderableQuad renderable in sceneGraph.Entities.Query<RenderableQuad>())
        {
            renderable.SetupBuffers(graphicsDevice);
        }
    }
}
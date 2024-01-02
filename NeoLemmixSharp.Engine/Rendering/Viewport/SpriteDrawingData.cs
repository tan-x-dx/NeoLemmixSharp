﻿using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.LevelBuilding.Sprites;

namespace NeoLemmixSharp.Engine.Rendering.Viewport;

public sealed class SpriteDrawingData
{
	private readonly int _originalSpriteWidth;
	private readonly int _originalSpriteHeight;

	private readonly PixelColorData _colorData;
	public DihedralTransformation DihedralTransformation { get; }
	public int ThisSpriteWidth { get; }
	public int ThisSpriteHeight { get; }

	public SpriteDrawingData(
		Orientation orientation,
		FacingDirection facingDirection,
		int originalSpriteWidth,
		int originalSpriteHeight,
		int numberOfFrames,
		int numberOfLayers)
	{
		_originalSpriteWidth = originalSpriteWidth;
		_originalSpriteHeight = originalSpriteHeight;

		var rotNum = orientation.RotNum;
		bool flipHorizontally;

		if ((rotNum & 1) != 0)
		{
			ThisSpriteWidth = _originalSpriteHeight;
			ThisSpriteHeight = _originalSpriteWidth;

			if (facingDirection == FacingDirection.LeftInstance)
			{
				rotNum = (rotNum + 2) & 3;
				flipHorizontally = true;
			}
			else
			{
				flipHorizontally = false;
			}
		}
		else
		{
			ThisSpriteWidth = _originalSpriteWidth;
			ThisSpriteHeight = _originalSpriteHeight;

			flipHorizontally = facingDirection == FacingDirection.LeftInstance;
		}

		var uints = new uint[originalSpriteWidth * originalSpriteHeight * numberOfFrames * numberOfLayers];
		_colorData = new PixelColorData(ThisSpriteWidth * numberOfLayers, ThisSpriteHeight * numberOfFrames, uints);
		DihedralTransformation = new DihedralTransformation(rotNum, flipHorizontally);
	}

	public void Set(uint pixel, int x0, int y0, int layer, int frame)
	{
		DihedralTransformation.Transform(
			x0,
			y0,
			_originalSpriteWidth - 1,
			_originalSpriteHeight - 1,
			out var x1,
			out var y1);

		var x2 = x1 + ThisSpriteWidth * layer;
		var y2 = y1 + ThisSpriteHeight * frame;
		_colorData.Set(x2, y2, pixel);
	}

	public Texture2D ToTexture(GraphicsDevice graphicsDevice)
	{
		var result = new Texture2D(graphicsDevice, _colorData.Width, _colorData.Height);
		result.SetData(_colorData.ColorData);
		return result;
	}
}
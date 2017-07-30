using System.Collections.Generic;

using pEngine;

using pEngine.Core.Graphics.Containers;
using pEngine.Core.Graphics.Drawables;
using pEngine.Core.Graphics.Textures;
using pEngine.Core.Graphics.Fonts;

using pEngine.Core.Audio;

using pEngine.Common.DataModel;
using pEngine.Common.Timing.Base;

namespace pEngineVisualText
{

	public class TestScene : Scene
	{
		Paragraph mamma;

        [LoaderFunction]
        private void Initializer(TextureStore Textures, FontStore Fonts)
        {
            #region Fonts

            DynamicFont comfortaaRegular120 = Fonts.GetFont("Resources/Comfortaa-Regular.ttf", 120);
            DynamicFont comfortaaRegular70 = new DynamicFont("Resources/Comfortaa-Regular.ttf", 70, false);
            DynamicFont comfortaaRegular40 = new DynamicFont("Resources/Comfortaa-Regular.ttf", 40, false);
            DynamicFont comfortaaLight = new DynamicFont("Resources/Comfortaa-Light.ttf", 70, false);
            DynamicFont comfortaaBold = new DynamicFont("Resources/Comfortaa-Bold.ttf", 120, false);

            comfortaaRegular120.Charsets.Add(Charset.BasicLatin);
            comfortaaRegular70.Charsets.Add(Charset.BasicLatin);
            comfortaaRegular40.Charsets.Add(Charset.BasicLatin);
            comfortaaLight.Charsets.Add(Charset.BasicLatin);
            comfortaaBold.Charsets.Add(Charset.BasicLatin);

			comfortaaRegular120.Load(Game);
            comfortaaRegular70.Load(Game);
			comfortaaRegular40.Load(Game);
            comfortaaLight.Load(Game);
            comfortaaBold.Load(Game);

            #endregion

            #region Textures

            StaticTextureAtlas a = new StaticTextureAtlas();
            a.LoadFromFiles
            (
                ("Background", "Resources/pEngineBackground.jpg")
            );
            a.Load(Game);

            #endregion

            #region Background

            Children.Add(new Sprite(a["Background"])
            {
                Size = Size,
                ScaleSize = pEngine.Core.Physics.Axes.Both,
                Stretch = StretchMode.UniformToFill

            }.Load(Game));

			#endregion

			#region pEngine title

			Children.Add((mamma = new Paragraph(comfortaaRegular120)
			{
				Text = $@"pEngine",
				Color = Color4.White,
				Position = new Vector2i(20, 30),
				ScaleWithParent = true
			}.Load<Paragraph>(Game)));

			Children.Add((mamma = new Paragraph(comfortaaLight)
			{
				Text = $@"2D game engine.",
				Color = Color4.White,
				Position = new Vector2i(580, 75),
				ScaleWithParent = true
			}.Load<Paragraph>(Game)));

			Children.Add((mamma = new Paragraph(comfortaaRegular40)
			{
				Text = $@"Developed by Andrea Demontis 2017",
				Color = Color4.White,
				Position = new Vector2i(20, 165),
                ScaleWithParent = true
			}.Load<Paragraph>(Game)));

            //mamma.Underline = new Underline()
            //{
            //	Enabled = false,
            //	Color = Color4.Aqua,
            //	Thickness = 1
            //};

            #endregion
        }


		public override void Update(IFrameBasedClock clock)
		{
			
			base.Update(clock);

		}
	}
}

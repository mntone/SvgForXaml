# Svg for Xaml Library

[![License](https://img.shields.io/github/license/mntone/SvgForXaml.svg?style=flat-square)](https://github.com/mntone/SvgForXaml/blob/master/LICENSE.txt)

Draw images from svg file with Win2D.

<img src="https://github.com/mntone/SvgForXaml/blob/master/images/ss.png" alt="app screenshot" width="464" height="526">

Tokyo railmaps

<img src="https://github.com/mntone/SvgForXaml/blob/master/images/ss2.png" alt="app screenshot" width="867" height="705">

Thanks to [railmaps](https://github.com/hashcc/railmaps)

## Requirement

- [Win2D](https://github.com/Microsoft/Win2D)


## Usage

### xaml with binding

Binding content type is `SvgDocument`.

```xml
<svg:SvgImage Content="{Binding ...}" />
```

### Renderer a raster image from svg image

```csharp
var svg = SvgDocument.Parse(...);
var uri = new Uri("ms-appdata:///local/filename.jpg");
var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
await SvgImageRenderer.RendererImageAsync(file, new SvgImageRendererSettings()
{
	Document = svg,
	Format = SvgImageRendererFileFormat.Jpeg,
	Scaling = 10.0F,
	Quality = 0.8F,
});
```

### Rendering in Win2D ecosystem

```csharp
var svg = SvgDocument.Parse(...);

var width = 24.0F;
var height = 24.0F;
var scaling = 3.0F;
using (var device = CanvasDevice.GetSharedDevice())
using (var offScreen = new CanvasRenderTarget(device, width, height, scaling * 96.0F))
using (var renderer = new Win2dRenderer(offScreen, svg))
using (var session = offScreen.CreateDrawingSession())
{
	session.Clear(Colors.Transparent);
	renderer.Renderer(width, height, session); // <- rendering svg content
}
```

## LICENSE

[MIT License](https://github.com/mntone/SvgForXaml/blob/master/LICENSE.txt)


## Author

- mntone<br>
	GitHub: https://github.com/mntone<br>
	Twitter: https://twitter.com/mntone (posted in Japanese; however, english is ok)
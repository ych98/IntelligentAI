using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Enumerations;

public class ImageGenerationEnum : Enumeration
{
    public static ImageGenerationEnum New = new ImageGenerationEnum(1, nameof(New), "新图片生成");

    public static ImageGenerationEnum Old = new ImageGenerationEnum(2, nameof(Old), "老照片修复");

    public static ImageGenerationEnum ColorUpdate = new ImageGenerationEnum(3, nameof(ColorUpdate), "色彩提升");

    public static ImageGenerationEnum Denoising = new ImageGenerationEnum(4, nameof(Denoising), "图像去噪");

    public static ImageGenerationEnum ResolutionRatioUpdate = new ImageGenerationEnum(5, nameof(ResolutionRatioUpdate), "分辨率提升");

    public ImageGenerationEnum(int id, string name, string description) : base(id, name, description) { }

    public static ImageGenerationEnum GetById(int id) => FromId<ImageGenerationEnum>(id);
    
    public static ImageGenerationEnum GetByName(string name) => FromName<ImageGenerationEnum>(name);
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace IntelligentAI.Records.Fanews;


public record AiImagesRequest(string Input, string[] Data);

public record AiVideoRequest(string Input, string Data);

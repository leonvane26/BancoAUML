#pragma checksum "C:\Users\vane_\OneDrive\Escritorio\BANCO(3) Jun29 con template\BANCO\BANCO\Views\Movimiento\PrintPDF.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0799c34f54eb2368fdabb293b3b7993a1a74519e"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Movimiento_PrintPDF), @"mvc.1.0.view", @"/Views/Movimiento/PrintPDF.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\vane_\OneDrive\Escritorio\BANCO(3) Jun29 con template\BANCO\BANCO\Views\_ViewImports.cshtml"
using BANCO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\vane_\OneDrive\Escritorio\BANCO(3) Jun29 con template\BANCO\BANCO\Views\_ViewImports.cshtml"
using BANCO.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0799c34f54eb2368fdabb293b3b7993a1a74519e", @"/Views/Movimiento/PrintPDF.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"87a29d5e8c19b3036da8c34cac19d58e682403ea", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Movimiento_PrintPDF : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<PrintPDFModel>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "C:\Users\vane_\OneDrive\Escritorio\BANCO(3) Jun29 con template\BANCO\BANCO\Views\Movimiento\PrintPDF.cshtml"
  
    Layout = "_Layout_pdf";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2>Historial de movimientos</h2>\r\n<br>\r\n<br />\r\n");
#nullable restore
#line 10 "C:\Users\vane_\OneDrive\Escritorio\BANCO(3) Jun29 con template\BANCO\BANCO\Views\Movimiento\PrintPDF.cshtml"
 if (Model.Movimientos != null && Model.Movimientos.Count > 0)
{



#line default
#line hidden
#nullable disable
            WriteLiteral("    <tr>\r\n        <td>\r\n            <h4><strong>N° de Cuenta:</strong> ");
#nullable restore
#line 16 "C:\Users\vane_\OneDrive\Escritorio\BANCO(3) Jun29 con template\BANCO\BANCO\Views\Movimiento\PrintPDF.cshtml"
                                          Write(Model.Cuenta.Numero);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h4>\r\n            <br>\r\n");
            WriteLiteral(@"        </td>
    </tr>
    <br>
    <table class=""table"">
        <thead>
            <tr>

                <th>
                    Fecha
                </th>
                <th>
                    Monto
                </th>
                <th>
                    Tipo
                </th>
");
            WriteLiteral("            </tr>\r\n        </thead>\r\n        <tbody>\r\n");
#nullable restore
#line 42 "C:\Users\vane_\OneDrive\Escritorio\BANCO(3) Jun29 con template\BANCO\BANCO\Views\Movimiento\PrintPDF.cshtml"
             foreach (var movimiento in Model.Movimientos)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\r\n                    <td>\r\n                        ");
#nullable restore
#line 46 "C:\Users\vane_\OneDrive\Escritorio\BANCO(3) Jun29 con template\BANCO\BANCO\Views\Movimiento\PrintPDF.cshtml"
                   Write(movimiento.Fecha);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 49 "C:\Users\vane_\OneDrive\Escritorio\BANCO(3) Jun29 con template\BANCO\BANCO\Views\Movimiento\PrintPDF.cshtml"
                   Write(movimiento.Monto.ToString("C"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 52 "C:\Users\vane_\OneDrive\Escritorio\BANCO(3) Jun29 con template\BANCO\BANCO\Views\Movimiento\PrintPDF.cshtml"
                   Write(movimiento.Tipo);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n");
#nullable restore
#line 55 "C:\Users\vane_\OneDrive\Escritorio\BANCO(3) Jun29 con template\BANCO\BANCO\Views\Movimiento\PrintPDF.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("        </tbody>\r\n    </table>\r\n");
#nullable restore
#line 58 "C:\Users\vane_\OneDrive\Escritorio\BANCO(3) Jun29 con template\BANCO\BANCO\Views\Movimiento\PrintPDF.cshtml"
}
else
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <p>No se encontraron movimientos.</p>\r\n");
#nullable restore
#line 62 "C:\Users\vane_\OneDrive\Escritorio\BANCO(3) Jun29 con template\BANCO\BANCO\Views\Movimiento\PrintPDF.cshtml"
}

#line default
#line hidden
#nullable disable
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<PrintPDFModel> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
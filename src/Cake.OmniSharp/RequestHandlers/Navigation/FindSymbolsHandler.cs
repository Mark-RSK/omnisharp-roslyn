using System.Composition;
using System.Threading.Tasks;
using Cake.OmniSharp.Extensions;
using OmniSharp.Mef;
using OmniSharp.Models;
using OmniSharp;
using OmniSharp.Roslyn.CSharp.Services.Navigation;

namespace Cake.OmniSharp.RequestHandlers.Navigation
{
    [OmniSharpHandler(OmnisharpEndpoints.FindSymbols, Constants.LanguageNames.Cake), Shared]
    public class FindSymbolsHandler : CakeRequestHandler<FindSymbolsRequest, QuickFixResponse>
    {
        [ImportingConstructor]
        public FindSymbolsHandler(OmniSharpWorkspace workspace)
            : base(workspace)
        {
        }

        protected override Task<QuickFixResponse> TranslateResponse(QuickFixResponse response, FindSymbolsRequest request)
        {
            return response.TranslateAsync(Workspace);
        }
    }
}

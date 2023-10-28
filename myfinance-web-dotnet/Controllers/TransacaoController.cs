using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using myfinance_web_dotnet.Models;
using myfinance_web_dotnet_domain.Entities;
using myfinance_web_dotnet_service.Interfaces;

namespace myfinance_web_dotnet.Controllers
{
    [Route("[controller]")]
    public class TransacaoController : Controller
    {
        private readonly ILogger<TransacaoController> _logger;
        private readonly ITransacaoService _transacaoService;
        private readonly IPlanoContaService _planoContaService;

        public TransacaoController(ILogger<TransacaoController> logger, ITransacaoService transacaoService, IPlanoContaService planoContaService)
        {
            _logger = logger;
            _transacaoService = transacaoService;
            _planoContaService = planoContaService;
        }

        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {            
            var listaTransacao = _transacaoService.ListarRegistros();
            List<TransacaoModel> listaTransacaoModel = new List<TransacaoModel>();

            foreach (var item in listaTransacao){
                var itemTransacao = new TransacaoModel(){
                    Id = item.Id,
                    Historico = item.Historico,
                    Valor = item.Valor,
                    Data = item.Data,
                    Tipo = item.PlanoConta.Tipo,
                    PlanoContaId = item.PlanoContaId
                };

                listaTransacaoModel.Add(itemTransacao);
            }

            ViewBag.ListaTransacao = listaTransacaoModel;

            return View();
        }

        [HttpGet]
        [Route("Cadastrar")]
        [Route("Cadastrar/{Id}")]
        public IActionResult Cadastrar(int? Id)
        {
            var listaPlanoContas = new SelectList(_planoContaService.ListarRegistros(), "Id", "Descricao");

            var itemTransacao = new TransacaoModel(){
                Data = DateTime.Now,
                ListaPlanoContas = listaPlanoContas
            };

            if(Id != null)
            {
                var transacao = _transacaoService.RetornarRegistro((int) Id);

                itemTransacao.Id = transacao.Id;
                itemTransacao.Historico = transacao.Historico;
                itemTransacao.Valor = transacao.Valor;
                itemTransacao.Data = transacao.Data;
                itemTransacao.PlanoContaId = transacao.PlanoContaId;
            }

            return View(itemTransacao);

        }

        [HttpPost]
        [Route("Cadastrar")]
        [Route("Cadastrar/{Id}")]
        public IActionResult Cadastrar(TransacaoModel model)
        {
            var transacao = new Transacao(){
                Id = model.Id,
                Historico = model.Historico,
                Valor = model.Valor,
                Data = model.Data,
                PlanoContaId = model.PlanoContaId
            };

            _transacaoService.Cadastrar(transacao);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Excluir/{Id}")]
        public IActionResult Excluir (int? Id)
        {
            _transacaoService.Excluir((int) Id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Error")]        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
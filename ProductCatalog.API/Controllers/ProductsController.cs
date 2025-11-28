using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;

namespace ProductCatalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        /// <summary>
        /// Lista todos os produtos, com opções de busca e ordenação.
        /// </summary>
        /// <param name="nomeBusca">Nome para busca parcial dos produtos.</param>
        /// <param name="ordenarPor">Campo pelo qual os produtos serão ordenados.</param>
        /// <param name="direcao">Direção da ordenação: ascendente ou descendente.</param>
        /// <returns>Retorna a lista de produtos filtrada e ordenada conforme os parâmetros fornecidos.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> Get(
            [FromQuery] string? nomeBusca,
            [FromQuery] string? ordenarPor,
            [FromQuery] string? direcao)
        {
            var produtos = await _productService.GetAllAsync(nomeBusca, ordenarPor, direcao);
            return Ok(produtos);
        }

        /// <summary>
        /// Obtém um produto pelo seu ID.
        /// </summary>
        /// <param name="id">ID do produto.</param>
        /// <returns>Retorna o produto correspondente ao ID fornecido.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponseDTO>> Get(int id)
        {
            var produto = await _productService.GetByIdAsync(id);
            if (produto == null)
            {
                return NotFound($"Produto com ID {id} não encontrado.");
            }
            return Ok(produto);
        }

        /// <summary>
        /// Adiciona um novo produto.
        /// </summary>
        /// <param name="produtoDto">Dados do produto a ser criado.</param>
        /// <returns>Retorna o produto criado.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Para erros de validação (Valor negativo)
        public async Task<ActionResult<ProductResponseDTO>> Post([FromBody] ProductCreateUpdateDTO produtoDto)
        {
            // O FluentValidation já interceptou o modelo.
            // Se o modelo for inválido (Valor < 0), o pipeline retorna 400 antes de chegar aqui.

            var novoProduto = await _productService.AddAsync(produtoDto);

            return CreatedAtAction(nameof(Get), new { id = novoProduto.Id }, novoProduto);
        }

        /// <summary>
        /// Atualiza um produto existente.
        /// </summary>
        /// <param name="id">ID do produto a ser atualizado.</param>
        /// <param name="produtoDto">Dados atualizados do produto.</param>
        /// <returns>Retorna status 204 No Content se a atualização for bem-sucedida.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] ProductCreateUpdateDTO produtoDto)
        {
            try
            {
                await _productService.UpdateAsync(id, produtoDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Exclui um produto pelo seu ID.
        /// </summary>
        /// <param name="id">ID do produto a ser excluído.</param>
        /// <returns>Retorna status 204 No Content se a exclusão for bem-sucedida.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productService.DeleteAsync(id);
                return NoContent(); // Retorno padrão para exclusão bem-sucedida
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Produto com ID {id} não encontrado.");
            }
        }
    }
}
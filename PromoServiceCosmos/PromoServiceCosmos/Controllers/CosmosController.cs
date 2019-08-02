using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoServiceCosmos.DataAccess;
using PromoServiceCosmos.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Linq;

namespace PromoServiceCosmos.Controllers
{
    [Route("/v1/promotions")]
    [ApiController]
    public class CosmosController : ControllerBase
    {
        ProductPromo productpromo = null;
        List<ProductPromo> promotions = null;
     //   public static string correlationalId = "X_CORRELATION";
        ICosmosDataAdapter _adapter;
        public CosmosController(ICosmosDataAdapter adapter)
        {
            _adapter = adapter;
        }

        /*[HttpGet("createdb")]
        public async Task<IActionResult> CreateDatabase()
        {
            var result = await _adapter.CreateDatabase("PromoDatabase");
            return Ok(result);
        }

        [HttpGet("createcollection")]
        public async Task<IActionResult> CreateCollection()
        {
            var result = await _adapter.CreateCollection("PromoDatabase", "PromoCollection");
            return Ok(result);
        }*/

        [HttpPost]
        public async Task<IActionResult> CreateDocument([FromBody] ProductPromo productpromo)
        {
            ResponseObject responseobject = new ResponseObject();
            try
            {
                var result = await _adapter.CreateDocument("PromoDatabase", "PromoCollection", productpromo);
                productpromo = result;
                responseobject.correlationalId = Guid.NewGuid().ToString();
                responseobject.statusCode = 201;
                responseobject.statusReason = "Created";
                responseobject.success = true;
                responseobject.promotionId = result.Id;
                return StatusCode(StatusCodes.Status201Created, responseobject);
            }
            catch (Exception ex)
            {
                responseobject.correlationalId = Guid.NewGuid().ToString();
                responseobject.statusCode = 400;
                responseobject.statusReason = "Bad Request";
                responseobject.success = false;
                return StatusCode(StatusCodes.Status400BadRequest, responseobject);
            }
         
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseObject responseobject = new ResponseObject();
            try
            {
                var result = await _adapter.DeleteUserAsync("PromoDatabase", "PromoCollection", id);
                 responseobject.correlationalId = Guid.NewGuid().ToString();
                    responseobject.statusCode = 202;
                    responseobject.statusReason = "Accepted";
                    responseobject.success = true;
                    return Ok(responseobject);
                
            }
            catch (Exception ex)
            {
                responseobject.correlationalId = Guid.NewGuid().ToString();
                responseobject.statusCode = 500;
                responseobject.statusReason = "Internal Server Error";
                responseobject.success = false;
                return StatusCode(StatusCodes.Status500InternalServerError, responseobject);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //List<ProductPromo> 
            //var  result = await _adapter.GetData("PromoDatabase", "PromoCollection");
            var result = await _adapter.GetData("PromoDatabase", "PromoCollection");
            //new System.Linq.SystemCore_EnumerableDebugView<object>(result).Items
           ResponseObject responseobject = new ResponseObject();
            // promotions = result;
             responseobject.correlationalId = Guid.NewGuid().ToString();
             responseobject.statusCode = 200;
             responseobject.statusReason = "OK";
             responseobject.success = true;

           responseobject.promotions = result ;
            return Ok(responseobject);



            // var list = JsonConvert.DeserializeObject<List<ProductPromo>>(result);

            /*  productpromo = result;
              ResponseObject responseobject = new ResponseObject();
                 List<ProductPromo> prmoList = new List<ProductPromo>();

              responseobject.correlationalId = Guid.NewGuid().ToString();
              responseobject.statusCode = 200;
              responseobject.statusReason = "OK";
              responseobject.success = true;

              responseobject.promotions = result;*/

            return Ok(result);

        }
       
        /* protected override void resolveCorrelationId(HttpServletRequest request)
         {
             // Look for the correlation ID in the request header
             String correlationId = request.getHeader(HEADER_FIELD_CORRELATION_ID);
             if (StringUtils.isBlank(correlationId))
             {
                 // If the request header does not have the correlation ID generate a new one for
                 // this request
                 correlationId = UUID.randomUUID().toString();
             }
             setCorrelationId(correlationId);
         }*/


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            
            ResponseObject responseobject = new ResponseObject();

            try
            {
                var result = await _adapter.GetDataById("PromoDatabase", "PromoCollection", id);

                productpromo = result;
                responseobject.correlationalId = Guid.NewGuid().ToString();
                responseobject.statusCode = 200;
                responseobject.statusReason = "OK";
                responseobject.success = true;

                responseobject.promotion = productpromo;
             return Ok(responseobject);

            }
            catch (Exception ex)
            {
               // ResponseObject responseobject = new ResponseObject();
                responseobject.correlationalId = Guid.NewGuid().ToString();
                responseobject.statusCode = 500;
                responseobject.statusReason = "Internal Server Error";
                responseobject.success = false;

                return StatusCode(StatusCodes.Status500InternalServerError, responseobject);

            }
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProductPromo productpromo)
        {
            ResponseObject responseobject = new ResponseObject();
            try
            {
                var result = await _adapter.updateDocumentAsync("PromoDatabase", "PromoCollection", productpromo);
              //  productpromo = result;
                responseobject.correlationalId = Guid.NewGuid().ToString();
                responseobject.statusCode = 202;
                responseobject.statusReason = "Accepted";
                responseobject.success = true;
               // responseobject.promotionId = result.Id;
                return StatusCode(StatusCodes.Status202Accepted, responseobject);
            }
            catch (Exception ex)
            {
                responseobject.correlationalId = Guid.NewGuid().ToString();
                responseobject.statusCode = 400;
                responseobject.statusReason = "Bad Request";
                responseobject.success = false;
                return StatusCode(StatusCodes.Status400BadRequest, responseobject);
            }
        }



        [HttpPost("{id}/conditions")]
        public async Task<IActionResult> CreateDocumentCondition(string id, [FromBody] ProductPromoCondition productPromoCondition)
        {
            ProductPromo productPromo = await _adapter.GetDataById("PromoDatabase", "PromoCollection", id);
            productPromo.conditions.Add(productPromoCondition);
            var result = await _adapter.CreateDocumentCondition("PromoDatabase", "PromoCollection", productPromo);
            return Ok(result);
        }


        [HttpPut("{id}/conditions/{index}")]
        public async Task<IActionResult> UpdateConditions(int index, string id, [FromBody] ProductPromoCondition productPromoCondition)
        {
            ProductPromo promo = await _adapter.GetDataById("PromoDatabase", "PromoCollection", id);
            var productPromoConditionInstance = promo.conditions.ElementAt(index);

            if (productPromoConditionInstance != null)
            {
                // productPromoConditionInstance.index = productPromoCondition.index;
                productPromoConditionInstance.parameter = productPromoCondition.parameter;
                productPromoConditionInstance.promoOperator = productPromoCondition.promoOperator;
                productPromoConditionInstance.conditionValue = productPromoCondition.conditionValue;
                productPromoConditionInstance.otherValue = productPromoCondition.otherValue;
            }
            else
            {
                return NotFound();
            }
            var result = await _adapter.UpdateDocumentAsyncCon("PromoDatabase", "PromoCollection", promo);
            return Ok(result);
        }


        [HttpGet("{id}/conditions/{index}")]
        public async Task<IActionResult> GetConditionByIndex(string id, int index)
        {
            ProductPromo promo = await _adapter.GetDataById("PromoDatabase", "PromoCollection", id);
            var result = promo.conditions.ElementAt(index);
            return Ok(result);
        }


        [HttpDelete("{id}/conditions/{index}")]
        public async Task<IActionResult> DeleteConditionByIndex(string id, int index)
        {
            ProductPromo promo = await _adapter.GetDataById("PromoDatabase", "PromoCollection", id);
            promo.conditions.RemoveAt(index);
            var result = await _adapter.UpdateDocumentAsyncCon("PromoDatabase", "PromoCollection", promo);
            return Ok(result);
        }


        [HttpGet("{id}/conditions")]
        public async Task<IActionResult> GetCondition(string id)
        {
            ProductPromo promo = await _adapter.GetDataById("PromoDatabase", "PromoCollection", id);
            var result = promo.conditions;
            return Ok(result);
        }


        [HttpDelete("{id}/conditions")]
        public async Task<IActionResult> DeleteConditions(string id)
        {
            ProductPromo promo = await _adapter.GetDataById("PromoDatabase", "PromoCollection", id);
            promo.conditions.Clear();
            var result = await _adapter.UpdateDocumentAsyncCon("PromoDatabase", "PromoCollection", promo);
            return Ok(result);
        }



        [HttpPost("{id}/actions")]
        public async Task<IActionResult> CreateActions(string id, [FromBody] ProductPromoAction productPromoAction)
        {



            ProductPromo productPromo = await _adapter.GetDataById("PromoDatabase", "PromoCollection", id);
            productPromo.actions = productPromoAction;
            var result = await _adapter.CreateAction("PromoDatabase", "PromoCollection", productPromo);
            return Ok(result);
        }



        [HttpPut("{id}/actions/")]
        public async Task<IActionResult> UpdateActions(string id, [FromBody] ProductPromoAction productPromoAction)
        {
            ProductPromo promo = await _adapter.GetDataById("PromoDatabase", "PromoCollection", id);
            var productPromoActionInstance = promo.actions;



            if (productPromoActionInstance != null)
            {
                productPromoActionInstance.type = productPromoAction.type;
                productPromoActionInstance.quantity = productPromoAction.quantity;
                productPromoActionInstance.amount = productPromoAction.amount;
                productPromoActionInstance.productId = productPromoAction.productId;
                productPromoActionInstance.catalogId = productPromoAction.catalogId;
            }
            else
            {
                return NotFound();
            }
            var result = await _adapter.UpdateActionDocumentAsync("PromoDatabase", "PromoCollection", promo);
            return Ok(result);
        }


    }
}

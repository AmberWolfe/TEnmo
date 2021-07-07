using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TransferController : ControllerBase
    {
        private static ITransferDAO transferDao;

        public TransferController(ITransferDAO _transferDao)
        {
            transferDao = _transferDao;       
        }
       
        [HttpGet("{userId}")]
        public List<Transfer> GetAllToAndFromTransfersForUser(int userId)
        {
            return transferDao.GetAllTransfersByUserId(userId);
        }
        
        [HttpGet("{userId}/{transferId}/details")]
        public Transfer GetTransferDetails(int transferId)
        {
            return transferDao.GetTransfer(transferId);
        }

        [HttpPost]
        public ActionResult<Transfer> AddTransfer(Transfer transfer)
        {            
            Transfer addedTransfer = transferDao.CreateTransfer(transfer);
            if (addedTransfer.TransferId == 0)
            {
                return BadRequest();
            }
            return Created($"/{addedTransfer.TransferId}", addedTransfer);
        }

        [HttpPut("{transferId}")]
        public ActionResult<Transfer> ApproveOrRejectPendingTransfer(int transferId, Transfer transfer)
        {
            Transfer transferCheck = transferDao.GetTransfer(transferId);
            if (transfer == null)
            {
                return NotFound();
            }
            else if (transferId != transfer.TransferId)
            {
                return BadRequest();
            }
            else
            {
                Transfer updateTransferStatus = transferDao.UpdateTransfer(transferId, transfer);
                return Ok(transfer);
            }  
        }


        [HttpPost("request")]
        public ActionResult<Transfer> RequestTransfer(Transfer transfer)
        {
            Transfer addedTransfer = transferDao.CreateTransferRequest(transfer);
            if (addedTransfer.TransferId == 0)
            {
                return BadRequest();
            }
            return Created($"/{addedTransfer.TransferId}", addedTransfer);
        }


    }
}

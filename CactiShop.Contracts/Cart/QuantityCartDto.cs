using System;
using System.Collections.Generic;
using System.Text;

namespace CactiShop.Contracts.Cart;

public record QuantityCartDto(int ProductId, int NewQuantity);
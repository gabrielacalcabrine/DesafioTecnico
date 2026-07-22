CREATE TABLE IF NOT EXISTS orders (
    "Id" uuid PRIMARY KEY,
    "Type" varchar(32) NOT NULL,
    "Asset" varchar(20) NOT NULL,
    "Quantity" integer NOT NULL CHECK ("Quantity" > 0),
    "Price" numeric(18,8) NOT NULL CHECK ("Price" > 0),
    "ExecutedQuantity" integer NOT NULL DEFAULT 0 CHECK ("ExecutedQuantity" >= 0),
    "Status" varchar(32) NOT NULL,
    "CreatedAt" timestamptz NOT NULL
);

CREATE TABLE IF NOT EXISTS trades (
    "Id" uuid PRIMARY KEY,
    "BuyOrderId" uuid NOT NULL REFERENCES orders("Id") ON DELETE RESTRICT,
    "SellOrderId" uuid NOT NULL REFERENCES orders("Id") ON DELETE RESTRICT,
    "Asset" varchar(20) NOT NULL,
    "Quantity" integer NOT NULL CHECK ("Quantity" > 0),
    "ExecutionPrice" numeric(18,8) NOT NULL CHECK ("ExecutionPrice" > 0),
    "ExecutedAt" timestamptz NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_orders_asset_status_price ON orders ("Asset", "Status", "Price");
CREATE INDEX IF NOT EXISTS ix_orders_created_at ON orders ("CreatedAt");
CREATE INDEX IF NOT EXISTS ix_trades_asset_executed_at ON trades ("Asset", "ExecutedAt");
CREATE INDEX IF NOT EXISTS ix_trades_buy_order_id ON trades ("BuyOrderId");
CREATE INDEX IF NOT EXISTS ix_trades_sell_order_id ON trades ("SellOrderId");

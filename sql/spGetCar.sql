
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES 
           WHERE SPECIFIC_SCHEMA = N'dbo'
                AND SPECIFIC_NAME = N'GetCar'
)
begin
    drop procedure dbo.GetCar
end
go

CREATE PROCEDURE dbo.GetCar
    @id int
AS
    select a.id, a.brand, a.[type], a.model, a.yom, a.color, a.imageUrl, a.price 
    from dbo.cars a
    where a.id = @id
GO

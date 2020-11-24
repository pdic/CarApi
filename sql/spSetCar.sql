
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES 
           WHERE SPECIFIC_SCHEMA = N'dbo'
                AND SPECIFIC_NAME = N'SetCar'
)
begin
    drop procedure dbo.setCar
end
go

CREATE PROCEDURE dbo.SetCar
    @status int,
    @id int,
    @brand varchar(50) = '',
    @type varchar(50) = '',
    @model varchar(50) = '',
    @yom int = 0,
    @color varchar(50) = '',
    @imageUrl varchar(200) = '',
    @price float = 0.0
AS
    /*New = 0,
      Valid = 1,
      Updated = 2,
      Deleted = 3*/

    if(@status = 0)
    begin
        insert into dbo.cars (brand, [type], model, yom, color, imageUrl, price )
        values(@brand, @type, @model, @yom, @color, @imageUrl, @price)
    end 
    else if(@status = 2) 
    begin
        update dbo.cars set 
            brand   = @brand, 
            [type]  = @type,
            model   = @model,
            yom     = @yom,
            color   = @color,
            imageUrl= @imageUrl,
            price   = @price
        where (id = @id)
    end
    else if (@status = 3)
    BEGIN
        delete dbo.cars where id = @id
    end

    select cast(SCOPE_IDENTITY() as int)
GO

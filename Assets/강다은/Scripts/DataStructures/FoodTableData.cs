 using BackEnd;
public class FoodTableData
{
	public string tableName = "FOODS";
	public string foodName;
	public int foodPrice;

	public FoodTableData() {}
	
	public FoodTableData(string foodName, int foodPrice)
	{
		this.foodName = foodName;
		this.foodPrice = foodPrice;
	}

	public Param ToParam()
	{
		Param param = new Param();
		param.Add("foodName", foodName);
		param.Add("foodPrice", foodPrice);
		return param;
	}
}


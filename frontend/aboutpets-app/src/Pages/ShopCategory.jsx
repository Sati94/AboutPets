
import SubCategory from '../Components/SubCategory/SubCategory';
import dog from '../Components/Assets/dog.png'
import cat from '../Components/Assets/cat.png'


const ShopCategory = ({ category }) => {

  const div = category === 1 ? <div><img src={dog} alt="Dog" style={{ width: '500px', height: '300px', display: 'block', margin: '50px auto' }} /></div> : <div><img src={cat} alt="Cat" style={{ width: '500px', height: '300px', display: 'block', margin: '50px auto' }} /></div>

  return (
    <div className="category-container">
      {div}
      <SubCategory category={category} />
    </div >
  );
};

export default ShopCategory;
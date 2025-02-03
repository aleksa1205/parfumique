import image from '../../../assets/images/forbidden.png';
import { Link } from 'react-router-dom';
import MainButton from '../../UiComponents/Buttons/MainButton';

export default function Forbidden() {
    return (
        <main className="flex flex-col items-center mb-16">
            <img src={image} alt="forbiden" className="max-w-2xl" />
            <h2 className='mb-5 font-bold text-2xl'>You are not allowed to enter this page</h2>
            <MainButton onClick={() => {}}>
                <Link to='/'>Back to home page</Link>
            </MainButton>
        </main>
    )
}
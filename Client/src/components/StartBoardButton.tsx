import { useNavigate } from "react-router-dom";
import { v4 as uuidv4 } from "uuid";

export default function StartBoardButton() {
    const navigate = useNavigate();

    const startBoard = () => {
        const newBoardId = uuidv4();
        navigate(`/${newBoardId}`);
    }

    return (
        <button 
            className="btn start-board" 
            onClick={() => startBoard()}
        >
            Start a board
        </button>
    );
}

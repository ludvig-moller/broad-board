import { useLocation } from "react-router-dom";

function Header() {
    const location = useLocation();
    const isBoardPage = location.pathname != "/";

    const copyBoardLink = () => {
        navigator.clipboard.writeText(window.location.href)
    }

    return (
        <header>
            { isBoardPage && <a href="/">Leave</a> }
            <h1>Broad Board</h1>
            { isBoardPage && <button onClick={copyBoardLink}>Copy Board Link</button>}
        </header>
    );
}

export default Header;

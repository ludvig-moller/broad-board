import { useLocation } from "react-router-dom";

function Header() {
    const location = useLocation();
    const isBoardPage = location.pathname != "/";

    const copyBoardLink = () => {
        navigator.clipboard.writeText(window.location.href);
    }

    return (
        <header className="header">
            <h1 className="title">BroadBoard</h1>
            { isBoardPage && <a className="btn home-link" href="/">Leave</a> }
            { isBoardPage && <button className="btn copy-board-link" onClick={copyBoardLink}>Copy Board Link</button>}
        </header>
    );
}

export default Header;

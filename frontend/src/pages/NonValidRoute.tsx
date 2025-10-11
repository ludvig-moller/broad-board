import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

export default function NonValidRoute() {
    const navigate = useNavigate();

    useEffect (() => {
        navigate("/");
    }, []);

    return <></>;
}

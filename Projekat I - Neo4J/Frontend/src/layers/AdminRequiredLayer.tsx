import { Outlet } from "react-router-dom";
import { Roles } from "../api/Roles";
import UseAuth from "../hooks/useAuth";
import Forbidden from "../components/ErrorBoundary/ErrorPages/Forbidden";

function AdminRequiredLayer() {
    const { auth } = UseAuth();

    if(auth.role !== Roles.Admin)
        return <Forbidden />
    else
        return <Outlet />
}

export default AdminRequiredLayer;
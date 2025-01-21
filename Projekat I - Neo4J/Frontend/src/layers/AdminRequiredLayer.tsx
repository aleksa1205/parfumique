import { Outlet } from "react-router-dom";
import { Roles } from "../api/Roles";
import UseAuth from "../hooks/useAuth";
import Forbidden from "../components/ErrorBoundary/ErrorPages/Forbidden";
import AdminFirstBlock from "../components/Admin/AdminFirstBlock";
import { useState } from "react";

export enum AdminPages {
    Dashboard,
    Fragrance,
    Note,
    Manufacturer,
    Parfumer,
    User
}

function AdminRequiredLayer() {
    const { auth } = UseAuth();
    const [activeAdminPage, setActiveAdminPage] = useState<AdminPages>(AdminPages.Dashboard);

    if(auth.role !== Roles.Admin)
        return <Forbidden />
    else {
        return (
            <>
                <AdminFirstBlock activeAdminPage={activeAdminPage} setActiveAdminPage={setActiveAdminPage} />
                <Outlet />
            </>
        )
    }
}

export default AdminRequiredLayer;
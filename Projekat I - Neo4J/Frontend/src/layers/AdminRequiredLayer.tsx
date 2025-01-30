import { Outlet, useLocation } from "react-router-dom";
import { Roles } from "../api/Roles";
import UseAuth from "../hooks/useAuth";
import Forbidden from "../components/ErrorBoundary/ErrorPages/Forbidden";
import AdminFirstBlock from "../components/Admin/AdminFirstBlock";
import { useEffect, useState } from "react";
import { stringToEnum } from "../utils";

export enum AdminPages {
    Dashboard,
    Fragrance,
    Note,
    Manufacturer,
    Perfumer,
    User
}

function AdminRequiredLayer() {
    const { auth } = UseAuth();
    
    const location = useLocation();
    const [activeAdminPage, setActiveAdminPage] = useState<AdminPages>(setActiveComponent(location.pathname)!);

    useEffect(() => {
        setActiveAdminPage(setActiveComponent!(location.pathname)); // Update the state in AdminRequiredLayer
    }, [location, activeAdminPage]);

    if(auth.role !== Roles.Admin)
        return <Forbidden />
    else {
        return (
            <>
                <AdminFirstBlock activeAdminPage={activeAdminPage} />
                <Outlet />
            </>
        )
    }
}

export default AdminRequiredLayer;

function setActiveComponent(location: string) {
    const lastParam = location.split('/').pop();

    if(lastParam === 'admin-dashboard' || lastParam === '') {
        return AdminPages.Dashboard
    }

    const result = stringToEnum(AdminPages, lastParam!)
    return result!;
}
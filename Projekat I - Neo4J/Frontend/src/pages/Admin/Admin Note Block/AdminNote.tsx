import { AdminActionCardContainer } from "../../../components/Admin/AdminActionCardContainer";
import { adminNoteCardsData } from "./AdminNoteCardsData";

export function AdminNotes() {
    return (
        <AdminActionCardContainer data={adminNoteCardsData} />
    )
}
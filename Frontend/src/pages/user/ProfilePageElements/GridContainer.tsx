export default function GridContainer({children}: {children:any}) {
  return (
    <div className="grid grid-cols-[2fr,5fr] gap-x-10">
      {children}
    </div>
  )
}
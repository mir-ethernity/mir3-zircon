import React, { FC, useMemo } from "react";
import { Typography, Link } from "@material-ui/core";

const Copyright: FC = () => {

    const year = useMemo(() => new Date().getFullYear(), []);

    return (
        <Typography variant="body2" color="textSecondary" align="center">
            &copy;&nbsp;
            <Link color="inherit" href="/">
                Ethernity Dashboard
            </Link>
            &nbsp;
            {year}
        </Typography>
    );
}

export default Copyright;